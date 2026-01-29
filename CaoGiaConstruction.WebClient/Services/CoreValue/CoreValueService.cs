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
    public interface ICoreValueService : IBaseService<CoreValue>
    {
        Task<Pager<CoreValue>> GetPaginationAsync(SearchKeywordPagination model);
        Task<OperationResult> AddOrUpdateActionAsync(CoreValueActionVM model);
        Task<OperationResult> UpdateSortOrderAsync(List<CoreValueSortDto> items);
        Task<List<CoreValue>> GetActiveCoreValuesAsync();
    }

    public class CoreValueService : BaseService<CoreValue>, ICoreValueService, ITransientService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public CoreValueService(AppDbContext context, IMapper mapper) : base(context)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Pager<CoreValue>> GetPaginationAsync(SearchKeywordPagination model)
        {
            var query = _context.CoreValues.AsNoTracking()
                 .Include(x => x.UserCreated)
                 .Where(x => x.IsDeleted != true)
                 .OrderBy(x => x.SortOrder).AsQueryable();
            if (!model.Keyword.IsNullOrEmpty())
            {
                model.Keyword = model.Keyword.ToLower().Trim();
                query = query.Where(x => x.Title.ToLower().Contains(model.Keyword) || 
                                        (x.Description != null && x.Description.ToLower().Contains(model.Keyword)));
            }
            return await query.ToPaginationAsync(model);
        }

        public async Task<OperationResult> AddOrUpdateActionAsync(CoreValueActionVM model)
        {
            var data = _mapper.Map<CoreValue>(model);

            if (data.SortOrder == 0)
            {
                var maxPosition = await _context.CoreValues.MaxAsync(x => (int?)x.SortOrder);
                data.SortOrder = (maxPosition ?? 0) + 1;
            }

            if (model.Id != Guid.Empty)
            {
                var exist = await _context.CoreValues.AsNoTracking().Where(x => x.Id == model.Id).FirstOrDefaultAsync();
                if (exist != null)
                {
                    // Preserve existing values
                    data.CreatedBy = exist.CreatedBy;
                    data.CreatedDate = exist.CreatedDate;
                    data.IsDeleted = exist.IsDeleted;
                    
                    // If Status is not provided in model, preserve existing Status
                    if (model.Status == null)
                    {
                        data.Status = exist.Status;
                    }
                    
                    // ModifiedBy and ModifiedDate will be set automatically by AppDbContext.AutoAddDateTracking()

                    _context.CoreValues.Update(data);
                }
                else
                {
                    // Set default Status if not provided
                    if (data.Status == null)
                    {
                        data.Status = StatusEnum.Active;
                    }
                    _context.CoreValues.Add(data);
                }
            }
            else
            {
                // Set default Status if not provided
                if (data.Status == null)
                {
                    data.Status = StatusEnum.Active;
                }
                _context.CoreValues.Add(data);
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

        public async Task<OperationResult> UpdateSortOrderAsync(List<CoreValueSortDto> items)
        {
            if (items == null || items.Count == 0)
            {
                return new OperationResult(StatusCodes.Status400BadRequest, MessageReponse.NOT_FOUND_DATA);
            }

            var ids = items.Select(x => x.Id).ToList();
            // Only update items that are not deleted
            var entities = await _context.CoreValues
                .Where(x => ids.Contains(x.Id) && x.IsDeleted != true)
                .ToListAsync();

            foreach (var entity in entities)
            {
                var item = items.FirstOrDefault(x => x.Id == entity.Id);
                if (item != null)
                {
                    entity.SortOrder = item.SortOrder;
                }
            }

            try
            {
                await _context.SaveChangesAsync();
                return new OperationResult(StatusCodes.Status200OK, MessageReponse.UPDATE_SUCCESS);
            }
            catch (Exception ex)
            {
                return ex.GetMessageError();
            }
        }

        public async Task<List<CoreValue>> GetActiveCoreValuesAsync()
        {
            var data = await _context.CoreValues
                .AsNoTracking()
                .Where(x => x.Status == StatusEnum.Active && x.IsDeleted != true)
                .OrderBy(x => x.SortOrder)
                .ToListAsync();

            return data;
        }

        public override async Task<OperationResult> RemoveAsync(Guid id)
        {
            var data = await _context.CoreValues.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (data != null)
            {
                // Soft delete: Set IsDeleted = true instead of removing from database
                data.IsDeleted = true;
                _context.CoreValues.Update(data);
                await _context.SaveChangesAsync();
                return new OperationResult(StatusCodes.Status200OK, MessageReponse.DELETE_SUCCESS);
            }
            return new OperationResult(StatusCodes.Status400BadRequest, MessageReponse.NOT_FOUND_DATA);
        }
    }
}
