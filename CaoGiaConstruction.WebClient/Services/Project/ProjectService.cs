using AutoMapper;
using Microsoft.EntityFrameworkCore;
using CaoGiaConstruction.Utilities;
using CaoGiaConstruction.Utilities.Constants;
using CaoGiaConstruction.Utilities.Dtos;
using CaoGiaConstruction.WebClient.Areas.Admin.Dtos;
using CaoGiaConstruction.WebClient.AutoMapper.ViewModels;
using CaoGiaConstruction.WebClient.Const;
using CaoGiaConstruction.WebClient.Context;
using CaoGiaConstruction.WebClient.Context.Entities;
using CaoGiaConstruction.WebClient.Context.Enums;
using CaoGiaConstruction.WebClient.Dtos;
using CaoGiaConstruction.WebClient.Extensions;
using CaoGiaConstruction.WebClient.Installers;

namespace CaoGiaConstruction.WebClient.Services
{
    public interface IProjectService : IBaseService<Project>
    {
        Task<Pager<ProjectNoContentVM>> GetPaginationAsync(SearchKeywordPagination model);

        Task<OperationResult> AddOrUpdateActionAsync(ProjectActionVM model);

        Task<Pager<ProjectNoContentVM>> GetPaginationProjectClientAsync(SearchPaginationDto model);

        Task<OperationResult> RemoveImageProjectAsync(Guid id, string path);

        Task<OperationResult> SortImageListAsync(Guid id, string imageList);

        Task<ProjectVM> FindProjectByCodeAsync(string code);

        Task<List<ProjectCategoryWithCountDto>> GetProjectCategoriesWithCountsAsync();

        Task<List<ProjectNoContentVM>> GetHotProjectsAsync(int count);

        Task<List<ProjectNoContentVM>> GetProjectRelatedsync(Guid catId, Guid id, int count = 10);

    }

    public class ProjectService : BaseService<Project>, IProjectService, ITransientService
    {
        private readonly IFileService _fileService;
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ProjectService(AppDbContext context, MapperConfiguration configMapper,
            IMapper mapper, IFileService fileService) : base(context)
        {
            _context = context;
            _mapper = mapper;
            _fileService = fileService;
        }

        public async Task<OperationResult> SortImageListAsync(Guid id, string imageList)
        {
            var data = await FindByIdAsync(id);
            if (data != null)
            {
                data.ImageList = imageList;
                _context.Update(data);
                try
                {
                    await _context.SaveChangesAsync();
                    return new OperationResult(StatusCodes.Status200OK, MessageReponse.ADD_OR_UPDATE_SUCCESS);
                }
                catch (Exception ex)
                {
                    return new OperationResult(StatusCodes.Status400BadRequest, ex.Message);
                }
            }
            else
            {
                return new OperationResult(StatusCodes.Status400BadRequest, MessageReponse.NOT_FOUND_DATA);
            }
        }

        public async Task<OperationResult> RemoveImageProjectAsync(Guid id, string path)
        {
            var data = await FindByIdAsync(id);
            if (data != null)
            {
                var resultDelete = await _fileService.DeleteFileAsync(path);
                if (resultDelete != null && resultDelete.Success)
                {
                    var listImage = data.ImageList.Split(";").ToList();
                    listImage.Remove(path);
                    var newImageList = String.Join(";", listImage);
                    data.ImageList = newImageList;
                    return await UpdateAsync(data);
                }
                else
                {
                    return resultDelete;
                }
            }
            else
            {
                return new OperationResult(StatusCodes.Status400BadRequest, MessageReponse.NOT_FOUND_DATA);
            }
        }


        public async Task<Pager<ProjectNoContentVM>> GetPaginationAsync(SearchKeywordPagination model)
        {
            var query = _context.Projects
                .Include(x =>x.Service)
                .Include(x => x.UserCreated)
                .OrderByDescending(x => x.CreatedDate)
                .AsNoTracking()
                .AsSplitQuery()
                .AsQueryable();

            if (!model.Keyword.IsNullOrEmpty())
            {
                model.Keyword = model.Keyword.ToLower().Trim();
                query = query.Where(x => x.Title.ToLower().Contains(model.Keyword));
            }

            return await query.Select(x => _mapper.Map<ProjectNoContentVM>(x)).ToPaginationAsync(model);
        }

        public async Task<OperationResult> AddOrUpdateActionAsync(ProjectActionVM model)
        {
            var data = _mapper.Map<Project>(model);

            #region Xử lý dữ cho liệu trường code
            string newCode = model.Title.ToUrlFormat();

            var isExistCode = await _context.Projects.CheckExistCodeAsync(newCode, model.Id.ToGuid());

            if (!isExistCode)
            {
                data.Code = model.Title.ToUrlFormat();
            }
            else
            {
                data.Code = newCode + "-" + RandomUtility.RandomString(6, 6);
            }
            #endregion

            #region Xử lý upload file
            // Upload single file (Avatar)
            bool isUploadFile = model.File != null && model.File.Length > 0;
            if (isUploadFile)
            {
                var fileResult = await _fileService.UploadImageWithExtensionWebpAsync(model.File, $"{Commons.FILE_UPLOAD}/project/");

                if (fileResult?.Success == true)
                {
                    data.Avatar = fileResult.Data.ToString();
                }
            }

            // Upload multiple files (ImageList)

            bool isUploadFiles = (model.Files != null && model.Files.Count() > 0);
            string imageList = string.Empty;

            if (isUploadFiles)
            {
                var fileResult = await _fileService.UploadFileMultipleAsync(model.Files, $"{Commons.FILE_UPLOAD}/project/");
                if (fileResult != null && fileResult.Success)
                {
                    imageList = fileResult.Data.ToString();
                }
            }
            #endregion

            if (model.Id != Guid.Empty)
            {
                var exist = await _context.Projects.AsNoTracking()
                                    .Where(x => x.Id == model.Id)
                                    .FirstOrDefaultAsync();
                if (exist != null)
                {
                    data.CreatedBy = exist.CreatedBy;
                    data.ModifiedBy = exist.ModifiedBy;
                    data.CreatedDate = exist.CreatedDate;
                    data.ModifiedDate = exist.ModifiedDate;
                    data.Code = exist.Code;
                    if (isUploadFiles)
                    {
                        var listOldImage = exist.ImageList.ToSafetyString().Split(";").ToList();
                        var listNewImage = imageList.ToSafetyString().Split(";").ToList();
                        var joinListImage = listOldImage.Concat(listNewImage);
                        data.ImageList = string.Join(";", joinListImage);
                    }
                    else
                    {
                        data.ImageList = exist.ImageList;
                    }

                    if (data.Avatar != exist.Avatar)
                    {
                        await _fileService.DeleteFileAsync(exist.Avatar);
                    }

                    _context.Projects.Update(data);
                }
                else
                {
                    data.ImageList = imageList;
                    _context.Projects.Add(data);
                }
            }
            else
            {
                data.ImageList = imageList;
                _context.Projects.Add(data);
            }
            try
            {
                await _context.SaveChangesAsync();
                return new OperationResult(StatusCodes.Status200OK, MessageReponse.ADD_OR_UPDATE_SUCCESS);
            }
            catch (Exception ex)
            {
                return new OperationResult(StatusCodes.Status400BadRequest, ex.Message);
            }
        }

        public override async Task<OperationResult> RemoveAsync(Guid id)
        {
            var data = await FindByIdAsync(id);
            if (data != null && !data.Avatar.IsNullOrEmpty())
            {
                await _fileService.DeleteFileAsync(data.Avatar);
            }
            if (data != null && !data.ImageList.IsNullOrEmpty())
            {
                await _fileService.DeleteMultipleFileAsync(data.ImageList);
            }

            return await base.RemoveAsync(id);
        }

        public async Task<Pager<ProjectNoContentVM>> GetPaginationProjectClientAsync(SearchPaginationDto model)
        {
            var query = _context.Projects
                .Include(x => x.Service)
                .Include(x => x.UserCreated)
                .Where(x => x.IsDeleted != true && x.Status == StatusEnum.Active)
                .OrderByDescending(x => x.CreatedDate)
                .AsNoTracking()
                .AsSplitQuery()
                .AsQueryable();

            if (!model.Keyword.IsNullOrEmpty())
            {
                model.Keyword = model.Keyword.ToLower().Trim();
                query = query.Where(x => x.Title.ToLower().Contains(model.Keyword));
            }

            if (!model.Code.IsNullOrEmpty())
            {
                query = query.Where(x => x.Service.Code == model.Code);
            }


            return await query.Select(x => _mapper.Map<ProjectNoContentVM>(x)).ToPaginationAsync(model);
        }

        public async Task<List<ProjectVM>> GetTopProjectsAsync(int count)
        {
            var Projects = await _context.Projects.AsNoTracking()
                .Where(x => x.Status == StatusEnum.Active)
                .Include(x => x.UserCreated)
                .OrderByDescending(x => x.CreatedDate)
                .Take(count)
                .ToListAsync();

            var ProjectVMs = _mapper.Map<List<ProjectVM>>(Projects);
            return ProjectVMs;
        }

        public async Task<ProjectVM> FindProjectByCodeAsync(string code)
        {
            var data = await _context.Projects
                .Include(x=>x.Service)
                .AsNoTracking()
                .AsSplitQuery()
                .FirstOrDefaultAsync();

            return _mapper.Map<ProjectVM>(data);
        }

        public async Task<List<ProjectCategoryWithCountDto>> GetProjectCategoriesWithCountsAsync()
        {
            var data = await _context.Services
               .AsNoTracking()
               .Include(x => x.Projects.Where(x => x.Status == StatusEnum.Active))
               .Where(x => x.IsDeleted != true && x.Status == StatusEnum.Active && x.Projects.Any(p => p.IsDeleted != true && p.Status == StatusEnum.Active))
               .OrderBy(x => x.SortOrder)
               .Select(x => new ProjectCategoryWithCountDto
               {

                   CountCategory = x.Projects.Count(),
                   Categories = x
               })
               .AsSplitQuery()
               .ToListAsync();

            return data;
        }

        public async Task<List<ProjectNoContentVM>> GetHotProjectsAsync(int count)
        {
            var blogs = await _context.Projects.AsNoTracking()
                .Include(x => x.Service)
                .Where(x => x.IsDeleted != true && x.Status == StatusEnum.Active && x.HotFlag == true)
                .OrderBy(x => Guid.NewGuid())
                .Take(count)
                .AsSplitQuery()
                .ToListAsync();
            return _mapper.Map<List<ProjectNoContentVM>>(blogs);
        }

        public async Task<List<ProjectNoContentVM>> GetProjectRelatedsync(Guid catId, Guid id, int count)
        {
            var data = await _context.Projects
                .Include(x => x.Service)
                .Where(x => x.ServiceId == catId && x.Id != id)
                .OrderByDescending(x => x.CreatedDate)
                .AsNoTracking()
                .AsSplitQuery()
                .Take(count)
                .ToListAsync();

            return _mapper.Map<List<ProjectNoContentVM>>(data);
        }
    }
}