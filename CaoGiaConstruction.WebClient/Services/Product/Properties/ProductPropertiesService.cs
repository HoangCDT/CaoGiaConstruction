using AutoMapper;
using CaoGiaConstruction.WebClient.Context;
using CaoGiaConstruction.WebClient.Context.Entities;
using CaoGiaConstruction.WebClient.Installers;

namespace CaoGiaConstruction.WebClient.Services
{
    public interface IProductPropertiesService : IBaseService<ProductProperties>
    {
    }

    public class ProductPropertiesService : BaseService<ProductProperties>, IProductPropertiesService, ITransientService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ProductPropertiesService(AppDbContext context, IMapper mapper) : base(context)
        {
            _context = context;
            _mapper = mapper;
        }
    }
}