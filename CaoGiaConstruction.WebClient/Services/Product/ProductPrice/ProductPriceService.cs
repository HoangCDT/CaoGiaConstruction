using AutoMapper;
using CaoGiaConstruction.WebClient.Context;
using CaoGiaConstruction.WebClient.Context.Entities;
using CaoGiaConstruction.WebClient.Installers;

namespace CaoGiaConstruction.WebClient.Services
{
    public interface IProductPriceService : IBaseService<ProductPrice>
    {
    }

    public class ProductPriceService : BaseService<ProductPrice>, IProductPriceService, ITransientService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ProductPriceService(AppDbContext context, IMapper mapper) : base(context)
        {
            _context = context;
            _mapper = mapper;
        }
    }
}