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
using CaoGiaConstruction.WebClient.Extensions;
using CaoGiaConstruction.WebClient.Installers;

namespace CaoGiaConstruction.WebClient.Services
{
    public interface ISlideService : IBaseService<Slide>
    {
        Task<Pager<Slide>> GetPaginationAsync(SearchSlideKeywordPagination model);

        Task<List<Slide>> GetSlideHomeAsync();

        Task<SlideVM> GetActiveSlideByCategoryCodeAsync(string categoryCode);

        Task<OperationResult> AddOrUpdateActionAsync(SlideActionVM model);
    }

    public class SlideService : BaseService<Slide>, ISlideService, ITransientService
    {
        private readonly AppDbContext _context;
        private readonly IFileService _fileService;
        private readonly IMapper _mapper;

        public SlideService(AppDbContext context, MapperConfiguration configMapper,
            IHttpContextAccessor contextAccessor, IMapper mapper, IFileService fileService) : base(context)
        {
            _context = context;
            _mapper = mapper;
            _fileService = fileService;
        }

        public async Task<Pager<Slide>> GetPaginationAsync(SearchSlideKeywordPagination model)
        {
            var query = _context.Slides.AsNoTracking()
                 .Include(x => x.UserCreated)
                   .Include(x => x.SlideCategory)
                 .OrderBy(x => x.SlideCategoryId).ThenBy(x => x.SortOrder).AsQueryable();
            if (!model.Keyword.IsNullOrEmpty())
            {
                model.Keyword = model.Keyword.ToLower().Trim();
                query = query.Where(x => x.Title.ToLower().Contains(model.Keyword));
            }
            if (model.SlideCategoryId != Guid.Empty)
            {
                query = query.Where(x => x.SlideCategoryId == model.SlideCategoryId);
            }
            return await query.ToPaginationAsync(model);
        }

        public async Task<OperationResult> AddOrUpdateActionAsync(SlideActionVM model)
        {
            var data = _mapper.Map<Slide>(model);

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
                var fileResult = await _fileService.UploadImageWithExtensionWebpAsync(model.File, $"{Commons.FILE_UPLOAD}/slide/");
                if (fileResult != null && fileResult.Success)
                {
                    data.Avatar = fileResult.Data.ToString();
                }
            }
            #endregion

            if (model.Id != Guid.Empty)
            {
                var exist = await _context.Slides.AsNoTracking().Where(x => x.Id == model.Id).FirstOrDefaultAsync();
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

                    _context.Slides.Update(data);
                }
                else
                {
                    _context.Slides.Add(data);
                }
            }
            else
            {
                if (data.SortOrder == null)
                {
                    var maxPosition = await _context.Slides.MaxAsync(x => x.SortOrder);
                    data.SortOrder = (maxPosition ?? 0) + 1;
                }
                _context.Slides.Add(data);
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

            return await base.RemoveAsync(id);
        }

        public async Task<List<Slide>> GetSlideHomeAsync()
        {
            var data = await _context.Slides.AsNoTracking()
                .Include(x => x.SlideCategory)
                .Where(x => x.Status == StatusEnum.Active)
                .OrderBy(x => x.SortOrder).ToListAsync();
            return data;
        }

        public async Task<SlideVM> GetActiveSlideByCategoryCodeAsync(string categoryCode)
        {
            var data = await _context.Slides
                    .AsNoTracking()
                    .Include(x => x.SlideCategory)
                    .Where(x => x.Status == StatusEnum.Active
                        && x.IsDeleted != true
                        && x.SlideCategory.Code == categoryCode)
                   .FirstOrDefaultAsync();

            return _mapper.Map<SlideVM>(data);
        }
    }
}