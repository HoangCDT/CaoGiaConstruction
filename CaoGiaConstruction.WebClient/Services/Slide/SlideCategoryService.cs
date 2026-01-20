using AutoMapper;
using Microsoft.EntityFrameworkCore;
using CaoGiaConstruction.Utilities;
using CaoGiaConstruction.Utilities.Dtos;
using CaoGiaConstruction.WebClient.Const;
using CaoGiaConstruction.WebClient.Context;
using CaoGiaConstruction.WebClient.Context.Entities;
using CaoGiaConstruction.WebClient.Context.Enums;
using CaoGiaConstruction.WebClient.Installers;

namespace CaoGiaConstruction.WebClient.Services
{
    public interface ISlideCategoryService : IBaseService<SlideCategory>
    {
        Task<List<SlideCategory>> GetSlideCategoryMenuAsnyc();

        Task<OperationResult> ChangeStatusAsync(Guid id, StatusEnum status);

        Task<string> FindCodeByIdAsync(Guid id);

        Task SortTableAsync(List<Guid> listId);
    }

    public class SlideCategoryService : BaseService<SlideCategory>, ISlideCategoryService, ITransientService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private OperationResult _operationResult;

        public SlideCategoryService(AppDbContext context, IMapper mapper) : base(context)
        {
            _context = context;
            _mapper = mapper;
            _operationResult = new OperationResult();
        }

        public async Task<List<SlideCategory>> GetSlideCategoryMenuAsnyc()
        {
            var query = await _context.SlideCategories.AsQueryable().AsNoTracking().ToListAsync();
            return await Task.FromResult(query);
        }

        public async Task<OperationResult> ChangeStatusAsync(Guid id, StatusEnum status)
        {
            var item = await _context.SlideCategories
                .AsNoTracking()
                .OrderByDescending(x => x.CreatedDate)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (item != null)
            {
                item.Status = status;
                try
                {
                    _context.Update(item);
                    await _context.SaveChangesAsync();
                    _operationResult = new OperationResult()
                    {
                        Success = true,
                        Data = item,
                        StatusCode = StatusCodes.Status200OK,
                        Message = MessageReponse.UPDATE_SUCCESS
                    };
                }
                catch (Exception ex)
                {
                    return ex.GetMessageError();
                }
            }
            else
            {
                _operationResult = new OperationResult()
                {
                    Success = false,
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = MessageReponse.NOT_FOUND_DATA
                };
            }
            return await Task.FromResult(_operationResult);
        }

        public async Task<string> FindCodeByIdAsync(Guid id)
        {
            string code = await _context.SlideCategories
                .Where(x => x.Id == id)
                .OrderByDescending(x => x.CreatedDate)
                .Select(x => x.Code)
                .FirstOrDefaultAsync();
            return code;
        }

        public async Task SortTableAsync(List<Guid> listId)
        {
            int i = 1;
            foreach (var id in listId)
            {
                var findItem = await _context.SlideCategories.FirstOrDefaultAsync(x => x.Id == id);
                if (findItem != null)
                {
                    findItem.SortOrder = i;
                    await _context.SaveChangesAsync();
                }
                i++;
            }
        }
    }
}