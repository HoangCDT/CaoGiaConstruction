using System.Linq.Expressions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using CaoGiaConstruction.Utilities;
using CaoGiaConstruction.Utilities.Constants;
using CaoGiaConstruction.Utilities.Dtos;
using CaoGiaConstruction.WebClient.Areas.Admin.Dtos;
using CaoGiaConstruction.WebClient.AutoMapper.ViewModels;
using CaoGiaConstruction.WebClient.Const;
using CaoGiaConstruction.WebClient.Context.Enums;
using CaoGiaConstruction.WebClient.Context;
using CaoGiaConstruction.WebClient.Context.Entities;
using CaoGiaConstruction.WebClient.Dtos;
using CaoGiaConstruction.WebClient.Extensions;
using CaoGiaConstruction.WebClient.Installers;

namespace CaoGiaConstruction.WebClient.Services
{
    public interface IBlogCategoryService : IBaseService<BlogCategory>
    {
        Task<Pager<BlogCategory>> GetPaginationAsync(SearchBlogCatKeywordPagination model);

        Task<OperationResult> AddOrUpdateActionAsync(BlogCategoryActionVM model);

        Task<BlogCategory> FindBlogCatgoryByCodeAsync(string code, BlogTypeEnum? type);

        Task<List<BlogCategoryWithCountDto>> GetBlogCategoriesWithCountsAsync(BlogTypeEnum? type);
    }

    public class BlogCategoryService : BaseService<BlogCategory>, IBlogCategoryService, ITransientService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IFileService _fileService;

        public BlogCategoryService(
            AppDbContext context,
            MapperConfiguration configMapper,
            IHttpContextAccessor contextAccessor,
            IMapper mapper,
            IFileService fileService
            ) : base(context)
        {
            _context = context;
            _mapper = mapper;
            _fileService = fileService;
        }

        public override IQueryable<BlogCategory> GetAll()
        {
            return base.AsQueryable().Where(x => x.Type == BlogTypeEnum.KNOWLEDGE);
        }

        public async Task<Pager<BlogCategory>> GetPaginationAsync(SearchBlogCatKeywordPagination model)
        {
            var query = _context.BlogCategories.AsNoTracking()
                  .Include(x => x.UserCreated)
                  .OrderBy(x => x.SortOrder)
                  .AsQueryable()
                  .AsSplitQuery()
                  .Where(x => model.Type == null || x.Type == model.Type); // Lấy dữ liệu Kiến thức

            if (!model.Keyword.IsNullOrEmpty())
            {
                model.Keyword = model.Keyword.ToLower().Trim();
                query = query.Where(x => x.Title.ToLower().Contains(model.Keyword));
            }

            return await query.ToPaginationAsync(model);
        }

        public async Task<OperationResult> AddOrUpdateActionAsync(BlogCategoryActionVM model)
        {
            var data = _mapper.Map<BlogCategory>(model);

            var folderName = model.Type switch
            {
                BlogTypeEnum.FORMULA => "formula-category",
                BlogTypeEnum.KNOWLEDGE => "knowledge-category",
                _ => "blog-category"
            };

            #region Xử lý dữ cho liệu trường code
            string newCode = model.Title.ToUrlFormat();

            var isExistCode = await _context.BlogCategories.CheckExistCodeAsync(newCode, model.Id.ToGuid());

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
                var fileResult = await _fileService.UploadImageWithExtensionWebpAsync(model.File, $"{Commons.FILE_UPLOAD}/{folderName}/");

                if (fileResult?.Success == true)
                {
                    data.Avatar = fileResult.Data.ToString();
                }
            }
            #endregion

            if (model.Id != Guid.Empty)
            {
                var exist = await _context.BlogCategories.AsNoTracking()
                            .Where(x => x.Id == model.Id)
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

                    _context.BlogCategories.Update(data);
                }
                else
                {
                    _context.BlogCategories.Add(data);
                }
            }
            else
            {
                if (data.SortOrder == null)
                {
                    var maxPosition = await _context.BlogCategories.Where(x=>x.Type == model.Type).MaxAsync(x => x.SortOrder);
                    data.SortOrder = (maxPosition ?? 0) + 1;
                }
                _context.BlogCategories.Add(data);
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

            // Check if data exists
            if (data == null)
            {
                return new OperationResult(StatusCodes.Status404NotFound, MessageReponse.NOT_FOUND_DATA);
            }

            // Check for foreign key constraints
            var hasRelatedBlogs = await _context.Blogs.AnyAsync(x => x.BlogCategoryId == id);

            if (hasRelatedBlogs)
            {
                return new OperationResult(StatusCodes.Status400BadRequest,
                    "Vui lòng xóa các bài viết trước khi xóa danh mục.");
            }

            // Handle deletion of file if necessary
            if (!data.Avatar.IsNullOrEmpty())
            {
                await _fileService.DeleteFileAsync(data.Avatar);
            }

            return await base.RemoveAsync(id);
        }

        public async Task<BlogCategory> FindBlogCatgoryByCodeAsync(string code, BlogTypeEnum? type)
        {
            var data = await _context.BlogCategories
                .AsNoTracking()
                .Where(x => x.Code == code)
                .Where(x => type == null || x.Type == type)
                .FirstOrDefaultAsync();
            return data;
        }

        public async Task<List<BlogCategoryWithCountDto>> GetBlogCategoriesWithCountsAsync(BlogTypeEnum? type)
        {
            var data = await _context.BlogCategories
               .AsNoTracking()
               .Include(x => x.Blogs.Where(x => x.Status == StatusEnum.Active))
               .Where(x => x.Status == StatusEnum.Active)
               .Where(x => type == null || x.Type == type)
                 .OrderBy(x => x.SortOrder)
               .Select(x => new BlogCategoryWithCountDto
               {

                   CountCategory = x.Blogs.Count(),
                   Categories = x
               })
               .AsSplitQuery()
               .ToListAsync();

            return data;
        }
    }
}