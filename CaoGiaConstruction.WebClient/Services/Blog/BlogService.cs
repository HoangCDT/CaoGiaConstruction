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
using CaoGiaConstruction.WebClient.Installers;
using CaoGiaConstruction.WebClient.Extensions;

namespace CaoGiaConstruction.WebClient.Services
{
    public interface IBlogService : IBaseService<Blog>
    {
        Task<List<BlogNoContentVM>> GetHotBlogHomeAsync(BlogTypeEnum? type);

        Task<BlogVM> FindBlogByCodeAsync(string code);

        Task<Pager<BlogNoContentVM>> GetPaginationAsync(SearchBlogCatKeywordPagination model);

        Task<OperationResult> AddOrUpdateActionAsync(BlogActionVM model);

        Task<Pager<BlogNoContentVM>> GetPaginationBlogClientAsync(SearchBlogClientDto model);

        Task<List<BlogNoContentVM>> GetBlogRelatedsync(Guid catId, Guid id, int count = 10);

        Task<List<BlogVM>> GetTopBlogsAsync(int count, BlogTypeEnum? type);

        Task<List<BlogNoContentVM>> GetTopHomeBlogsAsync(int count, BlogTypeEnum? type);

        Task<List<BlogNoContentVM>> GetHotBlogsAsync(int count, BlogTypeEnum? type);
    }

    public class BlogService : BaseService<Blog>, IBlogService, ITransientService
    {
        private readonly IFileService _fileService;
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public BlogService(AppDbContext context, MapperConfiguration configMapper,
            IMapper mapper, IFileService fileService) : base(context)
        {
            _context = context;
            _mapper = mapper;
            _fileService = fileService;
        }

        public async Task<BlogVM> FindBlogByCodeAsync(string code)
        {
            var data = await _context.Blogs
                .AsNoTracking()
                .Include(x => x.BlogCategory)
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Code == code);

            if (data == null)
            {
                return new BlogVM();
            }
            return _mapper.Map<BlogVM>(data);
        }

        public async Task<Pager<BlogNoContentVM>> GetPaginationAsync(SearchBlogCatKeywordPagination model)
        {
            var query = _context.Blogs.AsNoTracking()
                .Include(x => x.BlogCategory)
                .Include(x => x.UserCreated)
                .OrderByDescending(x => x.CreatedDate).AsQueryable()
                .Where(x => model.Type == null || x.Type == model.Type);

            if (!model.Keyword.IsNullOrEmpty())
            {
                model.Keyword = model.Keyword.ToLower().Trim();
                query = query.Where(x => x.Title.ToLower().Contains(model.Keyword));
            }

            if (model.BlogCategoryId != Guid.Empty)
            {
                query = query.Where(x => x.BlogCategoryId == model.BlogCategoryId);
            }

            return await query.Select(x => _mapper.Map<BlogNoContentVM>(x)).ToPaginationAsync(model);
        }

        public async Task<OperationResult> AddOrUpdateActionAsync(BlogActionVM model)
        {
            var data = _mapper.Map<Blog>(model);

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
            bool isUploadFile = model.File != null && model.File.Length > 0;
            if (isUploadFile)
            {
                var fileResult = await _fileService.UploadImageWithExtensionWebpAsync(model.File, $"{Commons.FILE_UPLOAD}/blog/");

                if (fileResult?.Success == true)
                {
                    data.Avatar = fileResult.Data.ToString();
                }
            }
            #endregion

            if (model.Id != Guid.Empty)
            {
                var exist = await _context.Blogs.AsNoTracking()
                                    .Where(x => x.Id == model.Id)
                                    .FirstOrDefaultAsync();
                if (exist != null)
                {
                    data.CreatedBy = exist.CreatedBy;
                    data.ModifiedBy = exist.ModifiedBy;
                    data.CreatedDate = exist.CreatedDate;
                    data.ModifiedDate = exist.ModifiedDate;
                    // Xóa ảnh cũ
                    if (data.Avatar != exist.Avatar)
                    {
                        await _fileService.DeleteFileAsync(exist.Avatar);
                    }
                    _context.Blogs.Update(data);
                }
                else
                {
                    _context.Blogs.Add(data);
                }
            }
            else
            {
                _context.Blogs.Add(data);
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
            if (data != null && !data.Avatar.IsNullOrEmpty() && data.Type == BlogTypeEnum.KNOWLEDGE)
            {
                await _fileService.DeleteFileAsync(data.Avatar);
            }
            return await base.RemoveAsync(id);
        }

        public async Task<List<BlogNoContentVM>> GetBlogRelatedsync(Guid catId, Guid id, int count)
        {
            var data = await _context.Blogs
                .AsNoTracking()
                .Include(x => x.BlogCategory)
                .Where(x => x.BlogCategory.Id == catId && x.Id != id)
                .OrderByDescending(x => x.CreatedDate)
                .Take(count)
                .AsSplitQuery()
                .ToListAsync();

            return _mapper.Map<List<BlogNoContentVM>>(data);
        }

        public async Task<Pager<BlogNoContentVM>> GetPaginationBlogClientAsync(SearchBlogClientDto model)
        {
            var query = _context.Blogs.AsNoTracking()
                .Include(x => x.BlogCategory)
                .Include(x => x.UserCreated)
                .Where(x => x.Status == StatusEnum.Active)
                .Where(x => model.Type == null || x.Type == model.Type)
                .OrderByDescending(x => x.CreatedDate)
                .AsQueryable();

            if (!model.Keyword.IsNullOrEmpty())
            {
                model.Keyword = model.Keyword.ToLower().Trim();
                query = query.Where(x => x.Title.ToLower().Contains(model.Keyword));
            }
            if (!model.Code.IsNullOrEmpty())
            {
                query = query.Where(x => x.BlogCategory.Code == model.Code);
            }

            return await query.Select(x => _mapper.Map<BlogNoContentVM>(x)).ToPaginationAsync(model);
        }

        public async Task<List<BlogVM>> GetTopBlogsAsync(int count, BlogTypeEnum? type)
        {
            var blogs = await _context.Blogs.AsNoTracking()
                .Where(x => x.Status == StatusEnum.Active)
                .Where(x => x.Type == null || x.Type == type)
                .Include(x => x.UserCreated)
                .Include(x => x.BlogCategory)
                .OrderByDescending(x => x.CreatedDate)
                .Take(count)
                .ToListAsync();

            var blogVMs = _mapper.Map<List<BlogVM>>(blogs);
            return blogVMs;
        }

        public async Task<List<BlogNoContentVM>> GetHotBlogHomeAsync(BlogTypeEnum? type)
        {
            var blogs = await _context.Blogs
                .Include(x => x.BlogCategory)
                .AsNoTracking()
                .Where(x => x.Status == StatusEnum.Active && x.HotFlag == true)
                .Where(x => x.HotFlag == true)
                .Where(x => x.Type == null || x.Type == type)
                .OrderBy(x => Guid.NewGuid())
                .AsSplitQuery()
                .ToListAsync();
            return _mapper.Map<List<BlogNoContentVM>>(blogs);
        }

        public async Task<List<BlogNoContentVM>> GetTopHomeBlogsAsync(int count, BlogTypeEnum? type)
        {
            var blogs = await _context.Blogs.AsNoTracking()
                .Include(x => x.BlogCategory)
                .Where(x => x.Status == StatusEnum.Active && x.HomeFlag == true)
                .OrderBy(x => Guid.NewGuid())
                .Take(count)

                .AsSplitQuery()
                .ToListAsync();
            return _mapper.Map<List<BlogNoContentVM>>(blogs);
        }

        public async Task<List<BlogNoContentVM>> GetHotBlogsAsync(int count, BlogTypeEnum? type)
        {
            var blogs = await _context.Blogs.AsNoTracking()
                .Include(x => x.BlogCategory)
                .Where(x => x.Status == StatusEnum.Active && x.HotFlag == true)
                .Where(x => x.BlogCategory.Type == type)
                .OrderBy(x => Guid.NewGuid())
                .Take(count)
                .AsSplitQuery()
                .ToListAsync();
            return _mapper.Map<List<BlogNoContentVM>>(blogs);
        }
    }
}