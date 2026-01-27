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
    public interface IServiceCategoryService : IBaseService<ServiceCategory>
    {
        Task<Pager<ServiceCategory>> GetPaginationAsync(SearchKeywordPagination model);

        Task<OperationResult> AddOrUpdateActionAsync(ServiceCategoryActionVM model);

        Task<List<ServiceCategory>> GetTopAsync(int count);

        Task<List<Dtos.ServiceCategorySortDto>> GetServiceCategorySortAsync();

        Task<ServiceCategory> FindServiceCatgoryByCodeAsync(string code);

        Task<OperationResult> UpdateSortOrderAsync(List<Areas.Admin.Dtos.ServiceCategorySortDto> items);
    }

    public class ServiceCategoryService : BaseService<ServiceCategory>, IServiceCategoryService, ITransientService
    {
        private readonly AppDbContext _context;
        private readonly IFileService _fileService;
        private readonly IMapper _mapper;

        public ServiceCategoryService(AppDbContext context,
           IMapper mapper, IFileService fileService) : base(context)
        {
            _context = context;
            _mapper = mapper;
            _fileService = fileService;
        }

        public async Task<Pager<ServiceCategory>> GetPaginationAsync(SearchKeywordPagination model)
        {
            var query = _context.ServiceCategories.AsNoTracking()
                 .Include(x => x.UserCreated)
                 .OrderBy(x => x.SortOrder).AsQueryable();
            if (!model.Keyword.IsNullOrEmpty())
            {
                model.Keyword = model.Keyword.ToLower().Trim();
                query = query.Where(x => x.Title.ToLower().Contains(model.Keyword));
            }
            return await query.ToPaginationAsync(model);
        }

        public async Task<OperationResult> AddOrUpdateActionAsync(ServiceCategoryActionVM model)
        {
            var data = _mapper.Map<ServiceCategory>(model);

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

            bool isUploadFile = (model.File != null && model.File.Length > 0);
            if (isUploadFile)
            {
                var fileResult = await _fileService.UploadImageWithExtensionWebpAsync(model.File, $"{Commons.FILE_UPLOAD}/service-category/");
                if (fileResult != null && fileResult.Success)
                {
                    data.Avatar = fileResult.Data.ToString();
                }
            }
            #endregion

            if (model.Id != Guid.Empty)
            {
                var exist = await _context.ServiceCategories.AsNoTracking().Where(x => x.Id == model.Id).FirstOrDefaultAsync();
                if (exist != null)
                {
                    data.CreatedBy = exist.CreatedBy;
                    data.ModifiedBy = exist.ModifiedBy;
                    data.CreatedDate = exist.CreatedDate;
                    data.ModifiedDate = exist.ModifiedDate;
                    data.Code = exist.Code;
                    if (data.Avatar != exist.Avatar)
                    {
                        await _fileService.DeleteFileAsync(exist.Avatar);
                    }

                    _context.ServiceCategories.Update(data);
                }
                else
                {
                    _context.ServiceCategories.Add(data);
                }
            }
            else
            {
                if (data.SortOrder == null)
                {
                    var maxPosition = await _context.ServiceCategories.MaxAsync(x => x.SortOrder);
                    data.SortOrder = (maxPosition ?? 0) + 1;
                }
                _context.ServiceCategories.Add(data);
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
            if (data != null && !data.Avatar.IsNullOrEmpty())
            {
                await _fileService.DeleteFileAsync(data.Avatar);
            }
            //Check FK
            if (data != null)
            {
                var isFK = await _context.Services.AnyAsync(x => x.ServiceCategoryId == id);
                if (isFK)
                {
                    return new OperationResult(StatusCodes.Status400BadRequest, "Danh mục sản phẩm bạn đang xóa đang chứa dữ liệu sản phẩm kèm theo! Vui lòng xóa sản phẩn liên quan trước khi xóa danh mục này");
                }
            }
            return await base.RemoveAsync(id);
        }

        public async Task<List<ServiceCategory>> GetTopAsync(int count)
        {
            var data = await _context.ServiceCategories
                .AsNoTracking()
                .Where(x => x.Status == StatusEnum.Active && x.IsDeleted != true)
                .OrderByDescending(x => x.CreatedDate)
                .Take(count)
                .OrderBy(x => x.SortOrder)
                .ToListAsync();

            return data;
        }

        public async Task<ServiceCategory> FindServiceCatgoryByCodeAsync(string code)
        {
            var data = await _context.ServiceCategories
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Code == code);
            return data;
        }

        public async Task<List<Dtos.ServiceCategorySortDto>> GetServiceCategorySortAsync()
        {
            var data = await _context.ServiceCategories
               .AsNoTracking()
               .Include(x => x.Services.Where(x => x.Status == StatusEnum.Active))
               .Where(x => x.Status == StatusEnum.Active)
                 .OrderBy(x => x.SortOrder)
               .Select(x => new Dtos.ServiceCategorySortDto
               {
                   CountCategory = x.Services.Count(),
                   ServiceCategory = x
               }).ToListAsync();

            return data;
        }

        public async Task<OperationResult> UpdateSortOrderAsync(List<Areas.Admin.Dtos.ServiceCategorySortDto> items)
        {
            if (items == null || items.Count == 0)
            {
                return new OperationResult(StatusCodes.Status400BadRequest, MessageReponse.NOT_FOUND_DATA);
            }

            var ids = items.Select(x => x.Id).ToList();
            var entities = await _context.ServiceCategories.Where(x => ids.Contains(x.Id)).ToListAsync();

            foreach (var entity in entities)
            {
                var item = items.FirstOrDefault(x => x.Id == entity.Id);
                if (item != null)
                {
                    entity.SortOrder = item.SortOrder;
                }
            }

            await _context.SaveChangesAsync();
            return new OperationResult(StatusCodes.Status200OK, MessageReponse.UPDATE_SUCCESS);
        }

    }
}