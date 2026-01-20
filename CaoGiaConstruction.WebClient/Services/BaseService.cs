using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using CaoGiaConstruction.Utilities;
using CaoGiaConstruction.Utilities.Dtos;
using CaoGiaConstruction.WebClient.Const;
using CaoGiaConstruction.WebClient.Context;
using CaoGiaConstruction.WebClient.Context.Enums;
using CaoGiaConstruction.WebClient.Context.Interface;

namespace CaoGiaConstruction.WebClient.Services
{
    public interface IBaseService<T>
    {
        Task<Pager<T>> GetPaginationAsync(BasePagination model, Expression<Func<T, bool>> predicate = null, bool isOrderByDescending = false);

        Task<T> FindByIdAsync(Guid id);

        Task<T> FindByIdAsync(Guid id, params Expression<Func<T, object>>[] includeProperties);

        Task<List<T>> GetAllAsync(Expression<Func<T, bool>> predicate = null);

        IQueryable<T> GetAll(Expression<Func<T, T>> expression);

        Task<OperationResult> RemoveAsync(Guid id);

        Task<OperationResult> RemoveMultipleAsync(List<T> entities);

        Task<OperationResult> AddAsync(T model);

        Task<OperationResult> AddOrUpdateAsync(T model);

        Task<OperationResult> AddRangerAsync(List<T> models);

        Task<OperationResult> UpdateAsync(T model);

        Task<OperationResult> ChangeStatus(Guid id);

        IQueryable<T> AsQueryable();
    }

    public class BaseService<T> : IBaseService<T> where T : class, IEntityBase<Guid>, IDateTracking, IActiveTracking
    {
        private readonly AppDbContext _context;

        public BaseService(AppDbContext context)
        {
            _context = context;
        }

        public virtual async Task<T?> FindByIdAsync(Guid id)
        {
            return await _context.Set<T>().AsNoTracking().Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public virtual async Task<T?> FindByIdAsync(Guid id, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> items = _context.Set<T>();
            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    items = items.Include(includeProperty);
                }
            }
            var query = await items.AsNoTracking().FirstOrDefaultAsync();
            return query;
        }

        public virtual async Task<List<T>> GetAllAsync(Expression<Func<T, bool>> predicate = null)
        {
            var query = _context.Set<T>().AsQueryable();
            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            return await query.AsNoTracking().ToListAsync();
        }

        public virtual IQueryable<T> GetAll()
        {
            var query = _context.Set<T>().AsQueryable();
            return query;
        }

        public virtual IQueryable<T> GetAll(Expression<Func<T, T>> expression)
        {
            var query = _context.Set<T>().Select(expression).AsQueryable().AsNoTracking();
            return query;
        }

        public virtual async Task<OperationResult> RemoveAsync(Guid id)
        {
            var data = await FindByIdAsync(id);
            if (data != null)
            {
                _context.Set<T>().Remove(data);
                await _context.SaveChangesAsync();
                return new OperationResult(StatusCodes.Status200OK, MessageReponse.DELETE_SUCCESS);
            }
            return new OperationResult(StatusCodes.Status400BadRequest, MessageReponse.NOT_FOUND_DATA);
        }

        public virtual async Task<OperationResult> RemoveMultipleAsync(List<T> entities)
        {
            try
            {
                _context.Set<T>().RemoveRange(entities);
                await _context.SaveChangesAsync();
                return new OperationResult(StatusCodes.Status200OK, MessageReponse.ADD_SUCCESS);
            }
            catch (Exception ex)
            {
                return ex.GetMessageError();
            }
        }

        public virtual async Task<OperationResult> AddAsync(T model)
        {
            try
            {
                _context.Set<T>().Add(model);
                await _context.SaveChangesAsync();
                return new OperationResult(StatusCodes.Status200OK, MessageReponse.ADD_SUCCESS, model.Id);
            }
            catch (Exception ex)
            {
                return ex.GetMessageError();
            }
        }

        public virtual async Task<OperationResult> AddRangerAsync(List<T> models)
        {
            try
            {
                _context.Set<T>().AddRange(models);
                await _context.SaveChangesAsync();
                return new OperationResult(StatusCodes.Status200OK, MessageReponse.ADD_SUCCESS);
            }
            catch (Exception ex)
            {
                return ex.GetMessageError();
            }
        }

        public virtual async Task<OperationResult> UpdateAsync(T model)
        {
            try
            {
                var data = await FindByIdAsync(model.Id);

                if (data != null)
                {
                    _context.Set<T>().Update(model);
                    await _context.SaveChangesAsync();
                    return new OperationResult(StatusCodes.Status200OK, MessageReponse.UPDATE_SUCCESS);
                }
                return new OperationResult(StatusCodes.Status400BadRequest, MessageReponse.NOT_FOUND_DATA);
            }
            catch (Exception ex)
            {
                return ex.GetMessageError();
            }
        }

        public virtual IQueryable<T> AsQueryable()
        {
            return _context.Set<T>().AsQueryable();
        }

        public virtual async Task<OperationResult> AddOrUpdateAsync(T model)
        {
            if (model.Id != Guid.Empty)
            {
                var exist = await _context.Set<T>().AsNoTracking().Where(x => x.Id == model.Id).FirstOrDefaultAsync();
                if (exist != null)
                {
                    _context.Set<T>().Update(model);
                }
                else
                {
                    _context.Set<T>().Add(model);
                }
            }
            else
            {
                _context.Set<T>().Add(model);
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

        public virtual async Task<Pager<T>> GetPaginationAsync(BasePagination model, Expression<Func<T, bool>> predicate = null, bool isOrderByDescending = false)
        {
            var query = _context.Set<T>().AsNoTracking();
            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            if (isOrderByDescending)
            {
                query = query.OrderByDescending(x => x.CreatedDate);
            }
            return await query.ToPaginationAsync(model);
        }

        public virtual async Task<OperationResult> ChangeStatus(Guid id)
        {
            var model = await FindByIdAsync(id);
            if (model == null)
            {
                return new OperationResult(StatusCodes.Status400BadRequest, MessageReponse.NOT_FOUND_DATA);
            }

            model.Status = (model.Status == StatusEnum.Active ? StatusEnum.InActive : StatusEnum.Active);
            _context.Set<T>().Update(model);
            await _context.SaveChangesAsync();
            return new OperationResult(StatusCodes.Status200OK, MessageReponse.ADD_OR_UPDATE_SUCCESS);
        }
    }
}