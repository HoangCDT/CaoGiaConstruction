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
    public interface IProductCategoryService : IBaseService<ProductCategory>
    {
        Task<Pager<ProductCategory>> GetPaginationAsync(SearchProductCatKeywordPagination model);

        Task<OperationResult> AddOrUpdateActionAsync(ProductCategoryActionVM model);

        Task<List<ProductCategory>> GetProductWithCategoryAsync();

        Task<List<ProductCategoryWithCountDto>> GetProductCategoriesWithCountsAsync();

        Task<ProductCategory> FindProductCatgoryByCodeAsync(string code);

        Task<List<ProductMainCategoryGroupVM>> GetWithProductMainCategoryGroupAsync();

        Task<List<ProductCategoryDto>> GetByProductMainCodeAsyncAsync(string mainCode);
    }

    public class ProductCategoryService : BaseService<ProductCategory>, IProductCategoryService, ITransientService
    {
        private readonly AppDbContext _context;
        private readonly IFileService _fileService;
        private readonly IMapper _mapper;

        public ProductCategoryService(AppDbContext context,
           IMapper mapper, IFileService fileService) : base(context)
        {
            _context = context;
            _mapper = mapper;
            _fileService = fileService;
        }

        public async Task<Pager<ProductCategory>> GetPaginationAsync(SearchProductCatKeywordPagination model)
        {
            var query = _context.ProductCategories.AsNoTracking()
                 .Include(x => x.UserCreated)
                 .OrderBy(x => x.SortOrder).AsQueryable();
            if (!model.Keyword.IsNullOrEmpty())
            {
                model.Keyword = model.Keyword.ToLower().Trim();
                query = query.Where(x => x.Title.ToLower().Contains(model.Keyword));
            }
            if (model.ProductMainCategoryId != null && model.ProductMainCategoryId != Guid.Empty)
            {
                query = query.Where(x => x.ProductMainCategoryId == model.ProductMainCategoryId);
            }
            return await query.ToPaginationAsync(model);
        }

        public async Task<OperationResult> AddOrUpdateActionAsync(ProductCategoryActionVM model)
        {
            var data = _mapper.Map<ProductCategory>(model);

            #region Xử lý dữ cho liệu trường code
            string newCode = model.Title.ToUrlFormat();

            var isExistCode = await _context.ProductCategories.CheckExistCodeAsync(newCode, model.Id.ToGuid());

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
            bool isUploadFile = model.File != null && model.File.Length > 0;
            if (isUploadFile)
            {
                var fileResult = await _fileService.UploadImageWithExtensionWebpAsync(model.File, $"{Commons.FILE_UPLOAD}/product-category/");

                if (fileResult?.Success == true)
                {
                    data.Avatar = fileResult.Data.ToString();
                }
            }
            #endregion

            if (model.Id != Guid.Empty)
            {
                var exist = await _context.ProductCategories.AsNoTracking()
                    .Include(x => x.ProductCategoryProperties)
                    .Where(x => x.Id == model.Id)
                    .AsSplitQuery()
                    .FirstOrDefaultAsync();
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

                    // Find the properties to remove
                    var newPropertyIds = data.ProductCategoryProperties.Select(p => p.PropertyId).ToList();
                    var propertiesToRemove = exist.ProductCategoryProperties
                                                  .Where(p => !newPropertyIds.Contains(p.PropertyId))
                                                  .ToList();

                    if (propertiesToRemove.Any())
                    {
                        _context.ProductCategoryProperties.RemoveRange(propertiesToRemove);
                    }

                    // Update the existing entity
                    _context.Entry(exist).CurrentValues.SetValues(data);
                    // Handle properties addition
                    foreach (var prop in data.ProductCategoryProperties)
                    {
                        var existingProperty = exist.ProductCategoryProperties.FirstOrDefault(p => p.PropertyId == prop.PropertyId);
                        if (existingProperty == null)
                        {
                            exist.ProductCategoryProperties.Add(prop);
                        }
                    }

                    _context.ProductCategories.Update(exist);
                }
                else
                {
                    _context.ProductCategories.Add(data);
                }
            }
            else
            {
                if (data.SortOrder == null)
                {
                    var maxPosition = await _context.ProductCategories.MaxAsync(x => x.SortOrder);
                    data.SortOrder = (maxPosition ?? 0) + 1;
                }
                _context.ProductCategories.Add(data);
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
                var isFK = await _context.Products.AnyAsync(x => x.ProductCategoryId == id);
                if (isFK)
                {
                    return new OperationResult(StatusCodes.Status400BadRequest, "Vui lòng xóa sản phẩm liên quan trước khi xóa danh mục này.");
                }
            }
            return await base.RemoveAsync(id);
        }

        public async Task<List<ProductCategory>> GetProductWithCategoryAsync()
        {
            var data = await _context.ProductCategories
                .AsNoTracking()
                .Include(x => x.Products.Where(x => x.Status == StatusEnum.Active)
                .OrderByDescending(x => x.CreatedDate).Take(10))
                .Where(x => x.Status == StatusEnum.Active && x.Products.Any())
                .OrderBy(x => x.SortOrder)
                .ToListAsync();
            return data;
        }

        public async Task<ProductCategory> FindProductCatgoryByCodeAsync(string code)
        {
            var data = await _context.ProductCategories
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Code == code);
            return data;
        }

        public async Task<List<ProductCategoryWithCountDto>> GetProductCategoriesWithCountsAsync()
        {
            var data = await _context.ProductCategories
               .AsNoTracking()
               .Include(x => x.Products.Where(x => x.Status == StatusEnum.Active))
               .Where(x => x.Status == StatusEnum.Active)
               .OrderBy(x => x.SortOrder)
               .Select(x => new ProductCategoryWithCountDto
               {
                   Categories = x,
                   CountCategory = x.Products.Count()

               })
               .AsSplitQuery()
               .ToListAsync();

            return data;
        }
        public override async Task<ProductCategory> FindByIdAsync(Guid id)
        {
            return base.AsQueryable().Include(x => x.ProductCategoryProperties).FirstOrDefault(x => x.Id == id);
        }

        public async Task<List<ProductMainCategoryGroupVM>> GetWithProductMainCategoryGroupAsync()
        {
            var groupedData = await _context.ProductCategories
                  .Where(x => x.Status == StatusEnum.Active
                    && x.IsDeleted != true
                    && x.ProductMainCategory.Status == StatusEnum.Active
                    && x.ProductMainCategory.IsDeleted != true)
                  .Include(x => x.ProductMainCategory)
                  .GroupBy(x => x.ProductMainCategory)
                  .Select(g => new ProductMainCategoryGroupVM
                  {
                      MainCategory = _mapper.Map<ProductMainCategoryVM>(g.Key),
                      Categories = g.Select(p => _mapper.Map<ProductCategoryVM>(p)).ToList()
                  })
                  .AsSplitQuery()
                  .ToListAsync();

            return groupedData;
        }

        public async Task<List<ProductCategoryDto>> GetByProductMainCodeAsyncAsync(string mainCode)
        {
            var data = await _context.ProductCategories
                   .Where(x => x.Status == StatusEnum.Active
                   && x.IsDeleted != true
                   && x.ProductMainCategory.Status == StatusEnum.Active
                   && x.ProductMainCategory.IsDeleted != true)
                  .Include(x => x.ProductMainCategory)
                  .GroupBy(x => x.ProductMainCategoryId)
                 .Select(x => new ProductCategoryDto
                 {
                     MainTitle = x.FirstOrDefault().ProductMainCategory.Title, // Assuming MainTitle is the title of the main category
                     Categories = _context.ProductCategories
                        .Include(x => x.ProductMainCategory)
                         .Where(pc => pc.ProductMainCategoryId == x.Key)
                         .Select(p => _mapper.Map<ProductCategoryVM>(p))
                         .ToList()
                 })
                 .AsSplitQuery()
                .ToListAsync();

            return data;
        }
    }
}