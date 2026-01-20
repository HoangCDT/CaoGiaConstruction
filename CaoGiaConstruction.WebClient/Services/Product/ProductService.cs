using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
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
    public interface IProductService : IBaseService<Product>
    {
        Task<List<ProductNoContentVM>> GetProductHomeAsync();

        Task<Pager<ProductNoContentVM>> GetPaginationAsync(SearchProductCatKeywordPagination model);

        Task<Pager<ProductNoContentVM>> GetPaginationProductClientAsync(SearchProductClientDto model);

        Task<OperationResult> AddOrUpdateActionAsync(ProductActionVM model);

        Task<OperationResult> RemoveImageProductAsync(Guid id, string path);

        Task<OperationResult> SortImageListAsync(Guid id, string imageList);

        Task<ProductVM> FindByIdIncludeAsyc(Guid id);

        Task<ProductVM> FindProductByCodeAsync(string code);

        Task<List<ProductNoContentVM>> GetProductRelatedsync(Guid catId, Guid id);

        Task<List<ProductNoContentVM>> GetProductsByMainCategoryCodeAsync(string? mainCode);

        Task<List<ProductGroupByCategoryVM>> GetGroupedProductsByCategoriesAsync(SearchProductClientDto model);
    }

    public class ProductService : BaseService<Product>, IProductService, ITransientService
    {
        private readonly IFileService _fileService;
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ProductService(AppDbContext context, IMapper mapper, IFileService fileService) : base(context)
        {
            _context = context;
            _mapper = mapper;
            _fileService = fileService;
        }

        public async Task<List<ProductNoContentVM>> GetProductHomeAsync()
        {
            var data = await _context.Products.AsNoTracking()
                .Where(x => x.Status == StatusEnum.Active)
                .OrderByDescending(x => x.CreatedDate)
                .Take(10).ToListAsync();
            return _mapper.Map<List<ProductNoContentVM>>(data);
        }

        public async Task<Pager<ProductNoContentVM>> GetPaginationAsync(SearchProductCatKeywordPagination model)
        {
            var query = _context.Products.AsNoTracking()
                .Include(x => x.ProductCategory)
                .Include(x => x.UserCreated)
                .OrderByDescending(x => x.CreatedDate)
                .AsSplitQuery()
                .AsQueryable();
            if (!model.Keyword.IsNullOrEmpty())
            {
                model.Keyword = model.Keyword.ToLower().Trim();
                query = query.Where(x => x.Title.ToLower().Contains(model.Keyword));
            }
            if (model.ProductCategoryId != Guid.Empty && model.ProductCategoryId != null)
            {
                query = query.Where(x => x.ProductCategoryId == model.ProductCategoryId);
            }
            if (model.ProductMainCategoryId != Guid.Empty && model.ProductMainCategoryId != null)
            {
                query = query.Where(x => x.ProductCategory.ProductMainCategoryId == model.ProductMainCategoryId);
            }

            return await query.Select(x => _mapper.Map<ProductNoContentVM>(x)).ToPaginationAsync(model);
        }

        public async Task<OperationResult> AddOrUpdateActionAsync(ProductActionVM model)
        {
            var data = _mapper.Map<Product>(model);

            // Handle code logic
            string newCode = model.Title.ToUrlFormat();
            var isExistCode = await _context.Blogs.CheckExistCodeAsync(newCode, model.Id.ToGuid());
            data.Code = isExistCode ? newCode + "-" + RandomUtility.RandomString(6, 6) : newCode;

            // Handle file uploads
            string imageList = string.Empty;
            string imageAvatar = string.Empty;
            if (model.File != null && model.File.Length > 0)
            {
                var fileResult = await _fileService.UploadImageWithExtensionWebpAsync(model.File, $"{Commons.FILE_UPLOAD}/product/");
                if (fileResult?.Success == true) imageAvatar = fileResult.Data.ToSafetyString();
            }
            if (model.Files != null && model.Files.Any())
            {
                var fileResult = await _fileService.UploadFileMultipleAsync(model.Files, $"{Commons.FILE_UPLOAD}/product/");
                if (fileResult?.Success == true) imageList = fileResult.Data.ToString();
            }

            // Check if updating or adding
            if (model.Id != Guid.Empty)
            {
                var exist = await _context.Products.FirstOrDefaultAsync(x => x.Id == model.Id);
                if (exist != null)
                {
                    // Update logic
                    data.CreatedBy = exist.CreatedBy;
                    data.CreatedDate = exist.CreatedDate;
                    if (!string.IsNullOrEmpty(imageList))
                    {
                        var listOldImage = exist.ImageList.ToSafetyString().Split(";").ToList();
                        var listNewImage = imageList.Split(";").ToList();
                        data.ImageList = string.Join(";", listOldImage.Concat(listNewImage));
                    }
                    else
                    {
                        data.ImageList = exist.ImageList;
                    }
                    data.Avatar = !string.IsNullOrEmpty(imageAvatar) ? imageAvatar : exist.Avatar;

                    // Update existing entity
                    _context.Entry(exist).CurrentValues.SetValues(data);
                }
                else
                {
                    return new OperationResult(StatusCodes.Status404NotFound, "Product not found.");
                }
            }
            else
            {
                // Add new entity
                data.Avatar = imageAvatar;
                data.ImageList = imageList;
                _context.Products.Add(data);
            }

            // Save changes and handle concurrency
            try
            {
                await _context.SaveChangesAsync();
                return new OperationResult(StatusCodes.Status200OK, MessageReponse.ADD_OR_UPDATE_SUCCESS);
            }
            catch (DbUpdateConcurrencyException)
            {
                return new OperationResult(StatusCodes.Status409Conflict, "Concurrency conflict detected.");
            }
            catch (Exception ex)
            {
                return ex.GetMessageError();
            }
        }

        public override async Task<OperationResult> RemoveAsync(Guid id)
        {
            var data = await FindByIdAsync(id);

            // check contact
            if (await _context.Contacts.AnyAsync(x => x.ProductId == id))
            {
                return new OperationResult(StatusCodes.Status400BadRequest,
                               "Sản phẩm bạn muốn xoá đang có Liên hệ! Hãy xoá liên hệ trước");
            }


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

        public async Task<OperationResult> RemoveImageProductAsync(Guid id, string path)
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

        public async Task<ProductVM> FindByIdIncludeAsyc(Guid id)
        {
            var data = await _context.Products.AsNoTracking().Include(x => x.ProductCategory).FirstOrDefaultAsync(x => x.Id == id);
            return _mapper.Map<ProductVM>(data);
        }

        public async Task<ProductVM> FindProductByCodeAsync(string code)
        {
            var data = await _context.Products.AsNoTracking()
                .Include(x => x.ProductCategory)
                .Include(x => x.ProductProperties)
                    .ThenInclude(x => x.Properties)
                .Include(x => x.ProductCategory)
                    .ThenInclude(x => x.ProductMainCategory)
                .AsSplitQuery()

                .FirstOrDefaultAsync(x => x.Code == code);
            return _mapper.Map<ProductVM>(data);
        }

        public async Task<List<ProductNoContentVM>> GetProductRelatedsync(Guid catId, Guid id)
        {
            var data = await _context.Products
                .AsNoTracking()
                .Include(x => x.ProductCategory)
                .Where(x => x.ProductCategory.Id == catId && x.Id != id)
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync();
            return _mapper.Map<List<ProductNoContentVM>>(data);
        }

        public async Task<Pager<ProductNoContentVM>> GetPaginationProductClientAsync(SearchProductClientDto model)
        {
            var query = _context.Products.AsNoTracking()
                .Include(x => x.ProductCategory)
                .Include(x => x.UserCreated)
                .Where(x => x.Status == StatusEnum.Active).AsQueryable();
            if (!model.Keyword.IsNullOrEmpty())
            {
                model.Keyword = model.Keyword.ToLower().Trim();
                query = query.Where(x => x.Title.ToLower().Contains(model.Keyword));
            }
            if (!model.Code.IsNullOrEmpty())
            {
                query = query.Where(x => x.ProductCategory.Code == model.Code);
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
                switch (model.OrderBy)
                {
                    case SortOrderConst.PriceAscending:
                        query = query.OrderBy(x => x.Price);
                        break;
                    case SortOrderConst.PriceDescending:
                        query = query.OrderByDescending(x => x.Price);
                        break;
                    case SortOrderConst.CreatedDateAscending:
                        query = query.OrderBy(x => x.CreatedDate);
                        break;
                    case SortOrderConst.CreatedDateDescending:
                        query = query.OrderByDescending(x => x.CreatedDate);
                        break;

                    default:
                        query = query.OrderByDescending(x => x.CreatedDate);
                        break;
                }
            }
            else
            {
                //Mặc định sắp xếp theo giá tăng dần
                query = query.OrderBy(x => x.Price);
            }
            return await query.Select(x => _mapper.Map<ProductNoContentVM>(x)).ToPaginationAsync(model);
        }

        public async Task<List<ProductGroupByCategoryVM>> GetGroupedProductsByCategoriesAsync(SearchProductClientDto model)
        {
            var query = _context.Products.AsNoTracking()
                .Include(x => x.ProductCategory)
                    .ThenInclude(x => x.ProductMainCategory)
                .Include(x => x.UserCreated)
                .Where(x => x.Status == StatusEnum.Active)
                .Where(x => model.ProductMainCategoryCode == null
                        || x.ProductCategory.ProductMainCategory.Code == model.ProductMainCategoryCode)

                .AsSplitQuery()
                .AsQueryable();

            if (!model.Keyword.IsNullOrEmpty())
            {
                model.Keyword = model.Keyword.ToLower().Trim();
                query = query.Where(x => x.Title.ToLower().Contains(model.Keyword));
            }
            if (!model.Code.IsNullOrEmpty() && model.Code != "all")
            {
                query = query.Where(x => x.ProductCategory.Code == model.Code);
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
                switch (model.OrderBy)
                {
                    case SortOrderConst.PriceAscending:
                        query = query.OrderBy(x => x.Price);
                        break;
                    case SortOrderConst.PriceDescending:
                        query = query.OrderByDescending(x => x.Price);
                        break;
                    case SortOrderConst.CreatedDateAscending:
                        query = query.OrderBy(x => x.CreatedDate);
                        break;
                    case SortOrderConst.CreatedDateDescending:
                        query = query.OrderByDescending(x => x.CreatedDate);
                        break;

                    default:
                        query = query.OrderByDescending(x => x.CreatedDate);
                        break;
                }
            }
            else
            {
                query = query.OrderBy(x => x.Price);
            }

            var groupedData = await query
                .GroupBy(x => x.ProductCategory)
                .Select(g => new ProductGroupByCategoryVM
                {
                    ProductCategory = _mapper.Map<ProductCategoryVM>(g.Key),
                    Products = g.Select(p => _mapper.Map<ProductNoContentVM>(p)).ToList()
                })
                .ToListAsync();

            // Assuming you have a method to paginate the result
            return groupedData;
        }

        public async Task<List<ProductNoContentVM>> GetProductsByMainCategoryCodeAsync(string? mainCode)
        {
            var products = await _context.Products.AsNoTracking()
                .Include(p => p.ProductCategory)
                    .ThenInclude(pc => pc.ProductMainCategory)
                .Where(p => p.Status == StatusEnum.Active && p.HotFlag == true)
                .Where(p => mainCode == null || p.ProductCategory.ProductMainCategory.Code == mainCode)
                .OrderByDescending(p => p.CreatedDate)
                .Take(5)
                .ToListAsync();

            return _mapper.Map<List<ProductNoContentVM>>(products);
        }
    }
}