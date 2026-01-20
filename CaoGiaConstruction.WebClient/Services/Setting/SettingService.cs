using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using CaoGiaConstruction.Utilities;
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
    public interface ISettingService : IBaseService<Setting>
    {
        Task<Pager<SettingVM>> GetPaginationAsync(SearchKeywordPagination model);

        Task<Setting> GetLatestSettingAsync();

        Task<SettingVM> GetSettingCacheAsync();

    }

    public class SettingService : BaseService<Setting>, ISettingService, ITransientService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IMemoryCache _memoryCache;
        private readonly IMapper _mapper;

        public SettingService(AppDbContext context, IMapper mapper,
            IHttpContextAccessor contextAccessor, IMemoryCache memoryCache) : base(context)
        {
            _context = context;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
            _memoryCache = memoryCache;
        }

        public async Task<Pager<SettingVM>> GetPaginationAsync(SearchKeywordPagination model)
        {
            var query = _context.Settings.AsNoTracking()
                .Include(x => x.UserCreated)
                .Include(x => x.UserModified)
                .AsQueryable();

            if (!_contextAccessor.HttpContext.User.HasAdminOrStaffRole())
            {
                query = query.Where(x => x.Status == StatusEnum.Active);
            }

            if (!model.Status.IsNullOrEmpty())
            {
                query = query.Where(x => x.Status == model.Status);
            }

            if (!model.Orderby.IsNullOrEmpty())
            {
                query = query.OrderByDescending(x => x.CreatedDate);
            }

            return await query.OrderByDescending(x => x.CreatedDate).Select(x => _mapper.Map<SettingVM>(x)).ToPaginationAsync(model);
        }

        public async Task<SettingVM> GetSettingCacheAsync()
        {
            // Define the cache expiration duration
            var cacheExpiration = DateTimeOffset.Now.AddMinutes(CacheConst.CACHE_MINUTE);

            // Get or create the cache entry
            var setting = await _memoryCache.GetOrCreateAsync(CacheConst.SETTING, async entry =>
            {
                entry.AbsoluteExpiration = cacheExpiration;
                return await _context.Settings
                    .AsNoTracking()
                    .OrderByDescending(x => x.CreatedDate)
                    .FirstOrDefaultAsync(x => x.Status == StatusEnum.Active && x.IsDeleted != true);
            });

            return _mapper.Map<SettingVM>(setting);
        }

        public async Task<Setting> GetLatestSettingAsync()
        {
            return await _context.Settings.OrderByDescending(x => x.CreatedDate).FirstOrDefaultAsync();
        }

        public override async Task<OperationResult> AddOrUpdateAsync(Setting model)
        {
            var data = await base.AddOrUpdateAsync(model);
            if (data.Success)
            {
                _memoryCache.Remove(CacheConst.SETTING);
            }
            return data;
        }

        public override async Task<OperationResult> ChangeStatus(Guid id)
        {
            var result = await base.ChangeStatus(id);
            if (result.Success)
            {
                _memoryCache.Remove(CacheConst.SETTING);
            }
            return result;
        }
    }
}