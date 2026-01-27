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
    public interface IProcessStepService : IBaseService<ProcessStep>
    {
        Task<Pager<ProcessStep>> GetPaginationAsync(SearchKeywordPagination model);

        Task<OperationResult> AddOrUpdateActionAsync(ProcessStepActionVM model);

        Task<OperationResult> UpdateSortOrderAsync(List<ProcessStepSortDto> items);

        Task<List<ProcessStep>> GetActiveProcessStepsAsync();
    }

    public class ProcessStepService : BaseService<ProcessStep>, IProcessStepService, ITransientService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ProcessStepService(AppDbContext context, IMapper mapper) : base(context)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Pager<ProcessStep>> GetPaginationAsync(SearchKeywordPagination model)
        {
            var query = _context.ProcessSteps.AsNoTracking()
                 .Include(x => x.UserCreated)
                 .OrderBy(x => x.SortOrder).AsQueryable();
            if (!model.Keyword.IsNullOrEmpty())
            {
                model.Keyword = model.Keyword.ToLower().Trim();
                query = query.Where(x => x.Title.ToLower().Contains(model.Keyword) || 
                                        (x.Description != null && x.Description.ToLower().Contains(model.Keyword)));
            }
            return await query.ToPaginationAsync(model);
        }

        public async Task<OperationResult> AddOrUpdateActionAsync(ProcessStepActionVM model)
        {
            var data = _mapper.Map<ProcessStep>(model);

            if (model.Id != Guid.Empty)
            {
                var exist = await _context.ProcessSteps.AsNoTracking().Where(x => x.Id == model.Id).FirstOrDefaultAsync();
                if (exist != null)
                {
                    data.CreatedBy = exist.CreatedBy;
                    data.ModifiedBy = exist.ModifiedBy;
                    data.CreatedDate = exist.CreatedDate;
                    data.ModifiedDate = exist.ModifiedDate;

                    _context.ProcessSteps.Update(data);
                }
                else
                {
                    _context.ProcessSteps.Add(data);
                }
            }
            else
            {
                if (data.SortOrder == null)
                {
                    var maxPosition = await _context.ProcessSteps.MaxAsync(x => (int?)x.SortOrder);
                    data.SortOrder = (maxPosition ?? 0) + 1;
                }
                _context.ProcessSteps.Add(data);
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

        public async Task<OperationResult> UpdateSortOrderAsync(List<ProcessStepSortDto> items)
        {
            if (items == null || items.Count == 0)
            {
                return new OperationResult(StatusCodes.Status400BadRequest, MessageReponse.NOT_FOUND_DATA);
            }

            var ids = items.Select(x => x.Id).ToList();
            var entities = await _context.ProcessSteps.Where(x => ids.Contains(x.Id)).ToListAsync();

            foreach (var entity in entities)
            {
                var item = items.FirstOrDefault(x => x.Id == entity.Id);
                if (item != null)
                {
                    entity.SortOrder = item.SortOrder;
                }
            }

            await _context.SaveChangesAsync();
            return new OperationResult(StatusCodes.Status200OK, MessageReponse.UPDATE_SUCCESS);
        }

        public async Task<List<ProcessStep>> GetActiveProcessStepsAsync()
        {
            var data = await _context.ProcessSteps
                .AsNoTracking()
                .Where(x => x.Status == StatusEnum.Active && x.IsDeleted != true)
                .OrderBy(x => x.SortOrder)
                .ToListAsync();

            return data;
        }
    }
}
