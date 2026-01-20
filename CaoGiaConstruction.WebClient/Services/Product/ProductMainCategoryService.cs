using AutoMapper;
using Microsoft.EntityFrameworkCore;
using CaoGiaConstruction.Utilities;
using CaoGiaConstruction.Utilities.Dtos;
using CaoGiaConstruction.WebClient.Areas.Admin.Dtos;
using CaoGiaConstruction.WebClient.AutoMapper.ViewModels;
using CaoGiaConstruction.WebClient.Const;
using CaoGiaConstruction.WebClient.Context;
using CaoGiaConstruction.WebClient.Context.Entities;
using CaoGiaConstruction.WebClient.Context.Enums;
using CaoGiaConstruction.WebClient.Dtos;
using CaoGiaConstruction.WebClient.Installers;

namespace CaoGiaConstruction.WebClient.Services
{
    public interface IProductMainCategoryService : IBaseService<ProductMainCategory>
    {
        Task<Pager<ProductMainCategoryVM>> GetPaginationAsync(SearchKeywordPagination model);
    }

    public class ProductMainCategoryService : BaseService<ProductMainCategory>,
        IProductMainCategoryService, ITransientService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ProductMainCategoryService(AppDbContext context,
           IMapper mapper) : base(context)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Pager<ProductMainCategoryVM>> GetPaginationAsync(SearchKeywordPagination model)
        {
            var query = _context.ProductCategories.AsNoTracking()
                 .Include(x => x.UserCreated)
                 .OrderBy(x => x.SortOrder).AsQueryable();
            if (!model.Keyword.IsNullOrEmpty())
            {
                model.Keyword = model.Keyword.ToLower().Trim();
                query = query.Where(x => x.Title.ToLower().Contains(model.Keyword));
            }
            return await query.Select(x => _mapper.Map<ProductMainCategoryVM>(x)).ToPaginationAsync(model);
        }
    }
}