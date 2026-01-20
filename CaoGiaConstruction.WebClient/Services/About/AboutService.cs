using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using CaoGiaConstruction.Utilities;
using CaoGiaConstruction.Utilities.Constants;
using CaoGiaConstruction.Utilities.Dtos;
using CaoGiaConstruction.WebClient.AutoMapper.ViewModels;
using CaoGiaConstruction.WebClient.Const;
using CaoGiaConstruction.WebClient.Context;
using CaoGiaConstruction.WebClient.Context.Entities;
using CaoGiaConstruction.WebClient.Installers;

namespace CaoGiaConstruction.WebClient.Services
{
    public interface IAboutService : IBaseService<About>
    {
        Task<AboutVM> GetAboutCacheAsync();

        Task<string> GetLogoTopCacheAsync();

        Task<AboutVM> GetLatestAboutAsync();

        Task<OperationResult> UpdateAboutAsync(AboutActionVM mode);
    }

    public class AboutService : BaseService<About>, IAboutService, ITransientService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _memoryCache;
        private readonly IFileService _fileService;

        public AboutService(AppDbContext context, IMapper mapper, IMemoryCache memoryCache, IFileService fileService) : base(context)
        {
            _context = context;
            _mapper = mapper;
            _memoryCache = memoryCache;
            _fileService = fileService;
        }


        public async Task<AboutVM> GetAboutCacheAsync()
        {
            // Định nghĩa thời gian hết hạn của cache
            var cacheExpiration = TimeSpan.FromMinutes(CacheConst.CACHE_MINUTE);

            // Lấy hoặc tạo cache entry
            var about = await _memoryCache.GetOrCreateAsync(CacheConst.ABOUT, async entry =>
            {
                // Đặt thời gian hết hạn tuyệt đối cho cache
                entry.AbsoluteExpirationRelativeToNow = cacheExpiration;

                // Lấy dữ liệu từ cơ sở dữ liệu
                var aboutEntity = await _context.Abouts
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

                return aboutEntity;
            });

            // Chuyển đổi dữ liệu entity sang ViewModel
            return _mapper.Map<AboutVM>(about);
        }

        public async Task<string> GetLogoTopCacheAsync()
        {
            // Định nghĩa thời gian hết hạn của cache
            var cacheExpiration = TimeSpan.FromMinutes(CacheConst.CACHE_MINUTE);

            // Lấy hoặc tạo cache entry
            var logo = await _memoryCache.GetOrCreateAsync(CacheConst.LOGO_TOP, async entry =>
            {
                // Đặt thời gian hết hạn tuyệt đối cho cache
                entry.AbsoluteExpirationRelativeToNow = cacheExpiration;

                // Lấy dữ liệu từ cơ sở dữ liệu
                var aboutEntity = await _context.Abouts
                    .AsNoTracking()
                    .Select(x => x.LogoTop)
                    .FirstOrDefaultAsync();

                return aboutEntity;
            });

            // Chuyển đổi dữ liệu entity sang ViewModel
            return logo;
        }

        public async Task<AboutVM> GetLatestAboutAsync()
        {
            var about = await _context.Abouts.FirstOrDefaultAsync();

            return _mapper.Map<AboutVM>(about);
        }

        public async Task<OperationResult> UpdateAboutAsync(AboutActionVM model)
        {
            var about = await _context.Abouts.AsNoTracking().FirstOrDefaultAsync();
            if (about != null)
            {
                var data = _mapper.Map<About>(model);
                //Upload logo top
                bool isUploadFileTop = (model.FileLogoTop != null && model.FileLogoTop.Length > 0);
                if (isUploadFileTop)
                {
                    var fileResult = await _fileService.UploadImageWithExtensionWebpAsync(model.FileLogoTop, $"{Commons.FILE_UPLOAD}/about/");
                    if (fileResult != null && fileResult.Success)
                    {
                        data.LogoTop = fileResult.Data.ToString();
                    }
                }
                //Upload logo Bottom
                bool isUploadFileBottom = (model.FileLogoBottom != null && model.FileLogoBottom.Length > 0);
                if (isUploadFileBottom)
                {
                    var fileResult = await _fileService.UploadImageWithExtensionWebpAsync(model.FileLogoBottom, $"{Commons.FILE_UPLOAD}/about/");
                    if (fileResult != null && fileResult.Success)
                    {
                        data.LogoBottom = fileResult.Data.ToString();
                    }
                }
                data.CreatedBy = about.CreatedBy;
                data.ModifiedBy = about.ModifiedBy;
                data.CreatedDate = about.CreatedDate;
                data.ModifiedDate = about.ModifiedDate;

                if (data.LogoTop != about.LogoTop)
                {
                    await _fileService.DeleteFileAsync(about.LogoTop);
                }

                if (data.LogoBottom != about.LogoBottom)
                {
                    await _fileService.DeleteFileAsync(about.LogoBottom);
                }

                _context.Abouts.Update(data);

                try
                {
                    await _context.SaveChangesAsync();
                    //remove cached
                    _memoryCache.Remove(CacheConst.ABOUT);
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
    }
}