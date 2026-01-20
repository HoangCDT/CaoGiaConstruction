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
using CaoGiaConstruction.WebClient.Installers;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace CaoGiaConstruction.WebClient.Services
{
    public interface IVideoService : IBaseService<Video>
    {
        Task<Pager<Video>> GetPaginationAsync(SearchKeywordPagination model);

        Task<OperationResult> AddOrUpdateActionAsync(VideoActionVM model);

    }

    public class VideoService : BaseService<Video>, IVideoService, ITransientService
    {
        private readonly AppDbContext _context;
        private readonly IFileService _fileService;
        private readonly IMapper _mapper;

        public VideoService(AppDbContext context,
           IMapper mapper, IFileService fileService) : base(context)
        {
            _context = context;
            _mapper = mapper;
            _fileService = fileService;
        }

        public async Task<Pager<Video>> GetPaginationAsync(SearchKeywordPagination model)
        {
            var query = _context.Videos.AsNoTracking()
                 .Include(x => x.UserCreated).AsQueryable();

            if (!model.Keyword.IsNullOrEmpty())
            {
                model.Keyword = model.Keyword.ToLower().Trim();
                query = query.Where(x => x.Title.ToLower().Contains(model.Keyword));
            }
            return await query.ToPaginationAsync(model);
        }

        public async Task<OperationResult> AddOrUpdateActionAsync(VideoActionVM model)
        {
            var data = _mapper.Map<Video>(model);

            bool isUploadFile = (model.File != null && model.File.Length > 0);
            if (isUploadFile)
            {
                var fileResult = await _fileService.UploadImageWithExtensionWebpAsync(model.File, $"{Commons.FILE_UPLOAD}/video/");
                if (fileResult != null && fileResult.Success)
                {
                    data.Thumbnail = fileResult.Data.ToString();
                }
            }
            if (model.Id != Guid.Empty)
            {
                var exist = await _context.Videos.AsNoTracking().Where(x => x.Id == model.Id).FirstOrDefaultAsync();
                if (exist != null)
                {
                    data.CreatedBy = exist.CreatedBy;
                    data.ModifiedBy = exist.ModifiedBy;
                    data.CreatedDate = exist.CreatedDate;
                    data.ModifiedDate = exist.ModifiedDate;

                    if (data.Thumbnail != exist.Thumbnail)
                    {
                        await _fileService.DeleteFileAsync(exist.Thumbnail);
                    }

                    _context.Entry(exist).CurrentValues.SetValues(data);

                    _context.Videos.Update(exist);
                }
                else
                {
                    _context.Videos.Add(data);
                }
            }
            else
            {
                _context.Videos.Add(data);
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
            if (data != null && !data.Thumbnail.IsNullOrEmpty())
            {
                await _fileService.DeleteFileAsync(data.Thumbnail);
            }

            return await base.RemoveAsync(id);
        }
    }
}