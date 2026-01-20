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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace CaoGiaConstruction.WebClient.Services
{
    public interface IServiceCaoGiaService : IBaseService<Service>
    {
        Task<List<ServiceNoContentVM>> GetTopServiceHomeAsync(int count = 3);

        Task<Pager<ServiceNoContentVM>> GetPaginationAsync(SearchServiceCatKeywordPagination model);

        Task<Pager<ServiceNoContentVM>> GetPaginationServiceClientAsync(SearchServiceClientDto model);

        Task<OperationResult> AddOrUpdateActionAsync(ServiceActionVM model);

        Task<OperationResult> RemoveImageServiceAsync(Guid id, string path);

        Task<OperationResult> SortImageListAsync(Guid id, string imageList);

        Task<ServiceVM> FindByIdIncludeAsyc(Guid id);

        Task<ServiceVM> FindServiceByCodeAsync(string code);

        Task<List<ServiceNoContentVM>> GetServiceRelatedsync(Guid catId, Guid id);

        Task<List<ServiceGroupByCategoryVM>> GetGroupedServiceByCategoriesAsync(SearchServiceClientDto model);
    }

    public class ServiceCaoGiaService : BaseService<Service>, IServiceCaoGiaService, ITransientService
    {
        private readonly IFileService _fileService;
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ServiceCaoGiaService(AppDbContext context, IMapper mapper, IFileService fileService) : base(context)
        {
            _context = context;
            _mapper = mapper;
            _fileService = fileService;
        }

        public async Task<List<ServiceNoContentVM>> GetTopServiceHomeAsync(int count)
        {
            var data = await _context.Services
                .Include(x => x.ServiceCategory)
                .Where(x => x.Status == StatusEnum.Active)
                .OrderByDescending(x => x.CreatedDate)
                .Take(count)
                .AsNoTracking()
                .ToListAsync();
            return _mapper.Map<List<ServiceNoContentVM>>(data);
        }
        public async Task<Pager<ServiceNoContentVM>> GetPaginationAsync(SearchServiceCatKeywordPagination model)
        {
            var query = _context.Services.AsNoTracking()
                .Include(x => x.ServiceCategory)
                .Include(x => x.UserCreated)
                .OrderByDescending(x => x.CreatedDate).AsQueryable();
            if (!model.Keyword.IsNullOrEmpty())
            {
                model.Keyword = model.Keyword.ToLower().Trim();
                query = query.Where(x => x.Title.ToLower().Contains(model.Keyword));
            }
            if (model.ServiceCategoryId != Guid.Empty)
            {
                query = query.Where(x => x.ServiceCategoryId == model.ServiceCategoryId);
            }
            return await query.Select(x => _mapper.Map<ServiceNoContentVM>(x)).ToPaginationAsync(model);
        }

        public async Task<OperationResult> AddOrUpdateActionAsync(ServiceActionVM model)
        {
            var data = _mapper.Map<Service>(model);
            #region Xử lý dữ cho liệu trường code
            string newCode = model.Title.ToUrlFormat();

            var isExistCode = await _context.Blogs.CheckExistCodeAsync(newCode, model.Id.ToGuid());

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
            string imageList = string.Empty;
            string imageAvatar = string.Empty;

            bool isUploadFiles = (model.Files != null && model.Files.Count() > 0);

            bool isUploadFile = model.File != null && model.File.Length > 0;

            if (isUploadFile)
            {
                var fileResult = await _fileService.UploadImageWithExtensionWebpAsync(model.File, $"{Commons.FILE_UPLOAD}/service/");
                if (fileResult != null && fileResult.Success)
                {
                    imageAvatar = fileResult.Data.ToSafetyString();
                }
            }


            if (isUploadFiles)
            {
                var fileResult = await _fileService.UploadFileMultipleAsync(model.Files, $"{Commons.FILE_UPLOAD}/service/");
                if (fileResult != null && fileResult.Success)
                {
                    imageList = fileResult.Data.ToString();
                }
            }

            #endregion

            if (model.Id != Guid.Empty)
            {
                var exist = await _context.Services.AsNoTracking().Where(x => x.Id == model.Id).FirstOrDefaultAsync();
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

                    if (isUploadFile)
                    {
                        data.Avatar = imageAvatar;
                    }
                    else
                    {
                        data.Avatar = exist.Avatar;
                    }



                    _context.Services.Update(data);
                }
                else
                {
                    _context.Services.Add(data);
                }
            }
            else
            {
                if (isUploadFiles)
                {
                    data.ImageList = imageList;
                }
                _context.Services.Add(data);
            }
            try
            {
                await _context.SaveChangesAsync();
                return new OperationResult(StatusCodes.Status200OK, MessageReponse.ADD_OR_UPDATE_SUCCESS);
            }
            catch (Exception ex)
            {
                return ex.GetMessageError();
            }
        }

        public override async Task<OperationResult> RemoveAsync(Guid id)
        {
            var data = await FindByIdAsync(id);
            if (data != null && !data.ImageList.IsNullOrEmpty())
            {
                await _fileService.DeleteMultipleFileAsync(data.ImageList);
            }

            return await base.RemoveAsync(id);
        }

        public async Task<OperationResult> RemoveImageServiceAsync(Guid id, string path)
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
                    return ex.GetMessageError();
                }
            }
            else
            {
                return new OperationResult(StatusCodes.Status400BadRequest, MessageReponse.NOT_FOUND_DATA);
            }
        }

        public async Task<ServiceVM> FindByIdIncludeAsyc(Guid id)
        {
            var data = await _context.Services.AsNoTracking().Include(x => x.ServiceCategory).FirstOrDefaultAsync(x => x.Id == id);
            return _mapper.Map<ServiceVM>(data);
        }

        public async Task<ServiceVM> FindServiceByCodeAsync(string code)
        {
            var data = await _context.Services
                .AsNoTracking()
                .Include(x => x.ServiceCategory)
                .FirstOrDefaultAsync(x => x.Code == code);

            return _mapper.Map<ServiceVM>(data);
        }

        public async Task<List<ServiceNoContentVM>> GetServiceRelatedsync(Guid catId, Guid id)
        {
            var data = await _context.Services
                .AsNoTracking()
                .Include(x => x.ServiceCategory)
                .Where(x => x.ServiceCategory.Id == catId && x.Id != id)
                .OrderByDescending(x => x.CreatedDate)
                .Take(10)
                .AsSplitQuery()
                .ToListAsync();
            return _mapper.Map<List<ServiceNoContentVM>>(data);
        }

        public async Task<Pager<ServiceNoContentVM>> GetPaginationServiceClientAsync(SearchServiceClientDto model)
        {
            var query = _context.Services.AsNoTracking()
                .Include(x => x.ServiceCategory)
                .Include(x => x.UserCreated)
                .Where(x => x.Status == StatusEnum.Active)
                .AsSplitQuery()
                .AsQueryable();
            if (!model.Keyword.IsNullOrEmpty())
            {
                model.Keyword = model.Keyword.ToLower().Trim();
                query = query.Where(x => x.Title.ToLower().Contains(model.Keyword));
            }
            if (!model.Code.IsNullOrEmpty())
            {
                query = query.Where(x => x.ServiceCategory.Code == model.Code);
            }

            if (model.PriceTo > 0)
            {
                query = query.Where(x => x.Price <= model.PriceTo);
            }
            if (model.PriceFrom > 0)
            {
                query = query.Where(x => x.Price >= model.PriceFrom);
            }

            if (!model.OrderBy.IsNullOrEmpty())
            {
                if (model.OrderBy == "asc")
                {
                    query = query.OrderBy(x => x.Price);
                }
                else if (model.OrderBy == "desc")
                {
                    query = query.OrderByDescending(x => x.Price);
                }
                if (model.OrderBy == "new")
                {
                    query = query.OrderByDescending(x => x.CreatedDate);
                }
                if (model.OrderBy == "old")
                {
                    query = query.OrderBy(x => x.CreatedDate);
                }
            }
            else
            {
                //Mặc định sắp xếp theo giá tăng dần
                query = query.OrderBy(x => x.Price);
            }
            return await query.Select(x => _mapper.Map<ServiceNoContentVM>(x)).ToPaginationAsync(model);
        }

        public async Task<List<ServiceGroupByCategoryVM>> GetGroupedServiceByCategoriesAsync(SearchServiceClientDto model)
        {
            var query = _context.Services.AsNoTracking()
                .Include(x => x.ServiceCategory)
                .Include(x => x.UserCreated)
                .Where(x => x.Status == StatusEnum.Active)
                .AsSplitQuery()
                .AsQueryable();

            if (!model.Keyword.IsNullOrEmpty())
            {
                model.Keyword = model.Keyword.ToLower().Trim();
                query = query.Where(x => x.Title.ToLower().Contains(model.Keyword));
            }
            if (!model.Code.IsNullOrEmpty())
            {
                query = query.Where(x => x.ServiceCategory.Code == model.Code);
            }
            if (model.PriceTo > 0)
            {
                query = query.Where(x => x.Price <= model.PriceTo);
            }
            if (model.PriceFrom > 0)
            {
                query = query.Where(x => x.Price >= model.PriceFrom);
            }
            if (!model.OrderBy.IsNullOrEmpty())
            {
                if (model.OrderBy == "asc")
                {
                    query = query.OrderBy(x => x.Price);
                }
                else if (model.OrderBy == "desc")
                {
                    query = query.OrderByDescending(x => x.Price);
                }
                if (model.OrderBy == "new")
                {
                    query = query.OrderByDescending(x => x.CreatedDate);
                }
                if (model.OrderBy == "old")
                {
                    query = query.OrderBy(x => x.CreatedDate);
                }
            }
            else
            {
                query = query.OrderBy(x => x.Price);
            }

            var groupedData = await query
                .GroupBy(x => x.ServiceCategory)
                .Select(g => new ServiceGroupByCategoryVM
                {
                    ServiceCategory = _mapper.Map<ServiceCategoryVM>(g.Key),
                    Service = g.Select(p => _mapper.Map<ServiceNoContentVM>(p)).ToList()
                })
                .ToListAsync();

            // Assuming you have a method to paginate the result
            return groupedData;
        }
    }
}