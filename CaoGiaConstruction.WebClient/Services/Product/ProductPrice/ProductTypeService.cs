using AutoMapper;
using CaoGiaConstruction.WebClient.Context;
using CaoGiaConstruction.WebClient.Context.Entities;
using CaoGiaConstruction.WebClient.Installers;

namespace CaoGiaConstruction.WebClient.Services
{
    public interface IProductTypeService : IBaseService<ProductType>
    {
    }

    public class ProductTypeService : BaseService<ProductType>, IProductTypeService, ITransientService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ProductTypeService(AppDbContext context, IMapper mapper) : base(context)
        {
            _context = context;
            _mapper = mapper;
        }
    }
}