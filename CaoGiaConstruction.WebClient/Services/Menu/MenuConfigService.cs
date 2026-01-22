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
    public interface IMenuConfigService : IBaseService<MenuConfig>
    {
        Task<Pager<MenuConfig>> GetPaginationAsync(SearchKeywordPagination model);

        Task<MenuConfig> GetActiveAsync();

        Task<OperationResult> SetActiveAsync(Guid id);

        Task<OperationResult> SetInactiveAsync(Guid id);
    }

    public class MenuConfigService : BaseService<MenuConfig>, IMenuConfigService, ITransientService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IMemoryCache _memoryCache;

        public MenuConfigService(AppDbContext context, IHttpContextAccessor contextAccessor, IMemoryCache memoryCache)
            : base(context)
        {
            _context = context;
            _contextAccessor = contextAccessor;
            _memoryCache = memoryCache;
        }

        public async Task<Pager<MenuConfig>> GetPaginationAsync(SearchKeywordPagination model)
        {
            var query = _context.MenuConfigs
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
                    (!string.IsNullOrEmpty(x.MenuKey) && x.MenuKey.Contains(model.Keyword)));
            }

            if (model.Status.HasValue)
            {
                query = query.Where(x => x.Status == model.Status);
            }

            return await query
                .OrderByDescending(x => x.CreatedDate)
                .ToPaginationAsync(model);
        }

        public async Task<MenuConfig> GetActiveAsync()
        {
            var cacheExpiration = DateTimeOffset.Now.AddMinutes(CacheConst.CACHE_MINUTE);
            return await _memoryCache.GetOrCreateAsync(CacheConst.MENU_CONFIG_ACTIVE, async entry =>
            {
                entry.AbsoluteExpiration = cacheExpiration;
                return await _context.MenuConfigs
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Status == StatusEnum.Active && x.IsDeleted != true);
            });
        }

        public async Task<OperationResult> SetActiveAsync(Guid id)
        {
            var entity = await _context.MenuConfigs.FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted != true);
            if (entity == null)
            {
                return new OperationResult(StatusCodes.Status400BadRequest, MessageReponse.NOT_FOUND_DATA);
            }

            var activeItems = await _context.MenuConfigs
                .Where(x => x.Id != id && x.Status == StatusEnum.Active && x.IsDeleted != true)
                .ToListAsync();

            foreach (var item in activeItems)
            {
                item.Status = StatusEnum.InActive;
            }

            entity.Status = StatusEnum.Active;
            await _context.SaveChangesAsync();
            _memoryCache.Remove(CacheConst.MENU_CONFIG_ACTIVE);
            return new OperationResult(StatusCodes.Status200OK, MessageReponse.UPDATE_SUCCESS);
        }

        public async Task<OperationResult> SetInactiveAsync(Guid id)
        {
            var entity = await _context.MenuConfigs.FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted != true);
            if (entity == null)
            {
                return new OperationResult(StatusCodes.Status400BadRequest, MessageReponse.NOT_FOUND_DATA);
            }

            entity.Status = StatusEnum.InActive;
            await _context.SaveChangesAsync();
            _memoryCache.Remove(CacheConst.MENU_CONFIG_ACTIVE);
            return new OperationResult(StatusCodes.Status200OK, MessageReponse.UPDATE_SUCCESS);
        }

        public override async Task<OperationResult> AddOrUpdateAsync(MenuConfig model)
        {
            if (model.Status == StatusEnum.Active)
            {
                var activeItems = await _context.MenuConfigs
                    .Where(x => x.Id != model.Id && x.Status == StatusEnum.Active && x.IsDeleted != true)
                    .ToListAsync();

                foreach (var item in activeItems)
                {
                    item.Status = StatusEnum.InActive;
                }
            }

            var result = await base.AddOrUpdateAsync(model);
            if (result.Success)
            {
                _memoryCache.Remove(CacheConst.MENU_CONFIG_ACTIVE);
            }
            return result;
        }

        public override async Task<OperationResult> ChangeStatus(Guid id)
        {
            var result = await base.ChangeStatus(id);
            if (result.Success)
            {
                _memoryCache.Remove(CacheConst.MENU_CONFIG_ACTIVE);
            }
            return result;
        }

        public override async Task<OperationResult> RemoveAsync(Guid id)
        {
            var result = await base.RemoveAsync(id);
            if (result.Success)
            {
                _memoryCache.Remove(CacheConst.MENU_CONFIG_ACTIVE);
            }
            return result;
        }
    }
}
