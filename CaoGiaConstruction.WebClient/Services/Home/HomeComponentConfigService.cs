using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using CaoGiaConstruction.Utilities;
using CaoGiaConstruction.WebClient.Areas.Admin.Dtos;
using CaoGiaConstruction.WebClient.Const;
using CaoGiaConstruction.WebClient.Context;
using CaoGiaConstruction.WebClient.Context.Entities;
using CaoGiaConstruction.WebClient.Context.Enums;
using CaoGiaConstruction.WebClient.Extensions;
using CaoGiaConstruction.WebClient.Installers;
using CaoGiaConstruction.Utilities.Dtos;

namespace CaoGiaConstruction.WebClient.Services
{
    public interface IHomeComponentConfigService : IBaseService<HomeComponentConfig>
    {
        Task<Pager<HomeComponentConfig>> GetPaginationAsync(SearchKeywordPagination model);

        Task<List<HomeComponentConfig>> GetActiveOrderedAsync();

        Task<OperationResult> UpdateSortOrderAsync(List<HomeComponentConfigSortDto> items);
    }

    public class HomeComponentConfigService : BaseService<HomeComponentConfig>, IHomeComponentConfigService, ITransientService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IMemoryCache _memoryCache;

        public HomeComponentConfigService(AppDbContext context, IHttpContextAccessor contextAccessor, IMemoryCache memoryCache)
            : base(context)
        {
            _context = context;
            _contextAccessor = contextAccessor;
            _memoryCache = memoryCache;
        }

        public async Task<Pager<HomeComponentConfig>> GetPaginationAsync(SearchKeywordPagination model)
        {
            var query = _context.HomeComponentConfigs
                .AsNoTracking()
                .Include(x => x.UserCreated)
                .Include(x => x.UserModified)
                .Where(x => x.IsDeleted != true)
                .AsQueryable();

            if (!_contextAccessor.HttpContext.User.HasAdminOrStaffRole())
            {
                query = query.Where(x => x.Status == StatusEnum.Active);
            }

            if (!string.IsNullOrWhiteSpace(model.Keyword))
            {
                query = query.Where(x =>
                    (!string.IsNullOrEmpty(x.Name) && x.Name.Contains(model.Keyword)) ||
                    (!string.IsNullOrEmpty(x.ComponentKey) && x.ComponentKey.Contains(model.Keyword)));
            }

            if (model.Status.HasValue)
            {
                query = query.Where(x => x.Status == model.Status);
            }

            return await query
                .OrderBy(x => x.SortOrder)
                .ThenByDescending(x => x.CreatedDate)
                .ToPaginationAsync(model);
        }

        public async Task<List<HomeComponentConfig>> GetActiveOrderedAsync()
        {
            var cacheExpiration = DateTimeOffset.Now.AddMinutes(CacheConst.CACHE_MINUTE);
            return await _memoryCache.GetOrCreateAsync(CacheConst.HOME_COMPONENT_CONFIGS, async entry =>
            {
                entry.AbsoluteExpiration = cacheExpiration;
                return await _context.HomeComponentConfigs
                    .AsNoTracking()
                    .Where(x => x.Status == StatusEnum.Active && x.IsDeleted != true)
                    .OrderBy(x => x.SortOrder)
                    .ToListAsync();
            });
        }

        public async Task<OperationResult> UpdateSortOrderAsync(List<HomeComponentConfigSortDto> items)
        {
            if (items == null || items.Count == 0)
            {
                return new OperationResult(StatusCodes.Status400BadRequest, MessageReponse.NOT_FOUND_DATA);
            }

            var ids = items.Select(x => x.Id).ToList();
            var entities = await _context.HomeComponentConfigs.Where(x => ids.Contains(x.Id)).ToListAsync();

            foreach (var entity in entities)
            {
                var item = items.FirstOrDefault(x => x.Id == entity.Id);
                if (item != null)
                {
                    entity.SortOrder = item.SortOrder;
                }
            }

            await _context.SaveChangesAsync();
            _memoryCache.Remove(CacheConst.HOME_COMPONENT_CONFIGS);
            return new OperationResult(StatusCodes.Status200OK, MessageReponse.UPDATE_SUCCESS);
        }

        public override async Task<OperationResult> AddOrUpdateAsync(HomeComponentConfig model)
        {
            if (!model.SortOrder.HasValue || model.SortOrder <= 0)
            {
                if (model.Id != Guid.Empty)
                {
                    var current = await _context.HomeComponentConfigs.AsNoTracking()
                        .FirstOrDefaultAsync(x => x.Id == model.Id);
                    if (current != null && current.SortOrder.HasValue)
                    {
                        model.SortOrder = current.SortOrder;
                    }
                }

                if (!model.SortOrder.HasValue || model.SortOrder <= 0)
                {
                    var maxOrder = await _context.HomeComponentConfigs.MaxAsync(x => (int?)x.SortOrder) ?? 0;
                    model.SortOrder = maxOrder + 1;
                }
            }

            var result = await base.AddOrUpdateAsync(model);
            if (result.Success)
            {
                _memoryCache.Remove(CacheConst.HOME_COMPONENT_CONFIGS);
            }
            return result;
        }

        public override async Task<OperationResult> ChangeStatus(Guid id)
        {
            var result = await base.ChangeStatus(id);
            if (result.Success)
            {
                _memoryCache.Remove(CacheConst.HOME_COMPONENT_CONFIGS);
            }
            return result;
        }

        public override async Task<OperationResult> RemoveAsync(Guid id)
        {
            var result = await base.RemoveAsync(id);
            if (result.Success)
            {
                _memoryCache.Remove(CacheConst.HOME_COMPONENT_CONFIGS);
            }
            return result;
        }
    }
}
