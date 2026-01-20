using AutoMapper;
using CaoGiaConstruction.WebClient.Context;
using CaoGiaConstruction.WebClient.Context.Entities;
using CaoGiaConstruction.WebClient.Installers;

namespace CaoGiaConstruction.WebClient.Services
{
    public interface IProductCategoryPropertiesService : IBaseService<ProductCategoryProperties>
    {
    }

    public class ProductCategoryPropertiesService : BaseService<ProductCategoryProperties>, IProductCategoryPropertiesService, ITransientService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ProductCategoryPropertiesService(AppDbContext context, IMapper mapper) : base(context)
        {
            _context = context;
            _mapper = mapper;
        }
    }
}