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
using CaoGiaConstruction.WebClient.Extensions;
using CaoGiaConstruction.WebClient.Installers;

namespace CaoGiaConstruction.WebClient.Services
{
    public interface IFeedbackService : IBaseService<Feedback>
    {
        Task<Pager<Feedback>> GetPaginationAsync(SearchKeywordPagination model);

        Task<OperationResult> AddOrUpdateActionAsync(FeedbackActionVM model);
    }

    public class FeedbackService : BaseService<Feedback>, IFeedbackService, ITransientService
    {
        private readonly AppDbContext _context;
        private readonly IFileService _fileService;
        private readonly IMapper _mapper;

        public FeedbackService(AppDbContext context, MapperConfiguration configMapper, IHttpContextAccessor contextAccessor,
            IMapper mapper, IFileService fileService) : base(context)
        {
            _context = context;
            _mapper = mapper;
            _fileService = fileService;
        }

        public async Task<Pager<Feedback>> GetPaginationAsync(SearchKeywordPagination model)
        {
            var query = _context.Feedbacks.AsNoTracking()
                 .Include(x => x.UserCreated)
                  .OrderBy(x => x.SortOrder)
                  .AsSplitQuery()
                  .AsQueryable();
            if (!model.Keyword.IsNullOrEmpty())
            {
                model.Keyword = model.Keyword.ToLower().Trim();
                query = query.Where(x => x.FullName.ToLower().Contains(model.Keyword));
            }
            return await query.ToPaginationAsync(model);
        }

        public async Task<OperationResult> AddOrUpdateActionAsync(FeedbackActionVM model)
        {
            var data = _mapper.Map<Feedback>(model);

            bool isUploadFile = (model.File != null && model.File.Length > 0);
            if (isUploadFile)
            {
                var fileResult = await _fileService.UploadImageWithExtensionWebpAsync(model.File, $"{Commons.FILE_UPLOAD}/feedback/");
                if (fileResult != null && fileResult.Success)
                {
                    data.Avatar = fileResult.Data.ToString();
                }
            }
            if (model.Id != Guid.Empty)
            {
                var exist = await _context.Feedbacks.AsNoTracking().Where(x => x.Id == model.Id).FirstOrDefaultAsync();
                if (exist != null)
                {
                    data.CreatedBy = exist.CreatedBy;
                    data.ModifiedBy = exist.ModifiedBy;
                    data.CreatedDate = exist.CreatedDate;
                    data.ModifiedDate = exist.ModifiedDate;
                    if (data.Avatar != exist.Avatar)
                    {
                        await _fileService.DeleteFileAsync(exist.Avatar);
                    }

                    _context.Feedbacks.Update(data);
                }
                else
                {
                    _context.Feedbacks.Add(data);
                }
            }
            else
            {
                if (data.SortOrder == null)
                {
                    var maxPosition = await _context.Feedbacks.MaxAsync(x => x.SortOrder);
                    data.SortOrder = (maxPosition ?? 0) + 1;
                }
                _context.Feedbacks.Add(data);
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
    }
}