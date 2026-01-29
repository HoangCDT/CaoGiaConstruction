using AutoMapper;
using Microsoft.EntityFrameworkCore;
using CaoGiaConstruction.Utilities;
using CaoGiaConstruction.Utilities.Constants;
using CaoGiaConstruction.Utilities.Dtos;
using CaoGiaConstruction.WebClient.Areas.Admin.Dtos;
using CaoGiaConstruction.WebClient.Const;
using CaoGiaConstruction.WebClient.Context;
using CaoGiaConstruction.WebClient.Context.Entities;
using CaoGiaConstruction.WebClient.Installers;

namespace CaoGiaConstruction.WebClient.Services
{
    public interface ITimeLineService : IBaseService<TimeLine>
    {
        Task<OperationResult> UpdateSortOrderAsync(List<TimeLineSortDto> items);
    }

    public class TimeLineService : BaseService<TimeLine>, ITimeLineService, ITransientService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public TimeLineService(AppDbContext context, IMapper mapper) : base(context)
        {
            _context = context;
            _mapper = mapper;
        }

        public override async Task<OperationResult> AddOrUpdateAsync(TimeLine model)
        {
            if (model.Id == Guid.Empty && model.SortOrder == 0)
            {
                var maxPosition = await _context.TimeLines.MaxAsync(x => (int?)x.SortOrder);
                model.SortOrder = (maxPosition ?? 0) + 1;
            }

            return await base.AddOrUpdateAsync(model);
        }

        public async Task<OperationResult> UpdateSortOrderAsync(List<TimeLineSortDto> items)
        {
            if (items == null || items.Count == 0)
            {
                return new OperationResult(StatusCodes.Status400BadRequest, "Dữ liệu không hợp lệ");
            }

            var ids = items.Select(x => x.Id).ToList();
            var entities = await _context.TimeLines.Where(x => ids.Contains(x.Id)).ToListAsync();

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
    }
}