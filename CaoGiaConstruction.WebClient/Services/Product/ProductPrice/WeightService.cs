using AutoMapper;
using CaoGiaConstruction.WebClient.Context;
using CaoGiaConstruction.WebClient.Context.Entities;
using CaoGiaConstruction.WebClient.Installers;

namespace CaoGiaConstruction.WebClient.Services
{
    public interface IWeightService : IBaseService<Weight>
    {
    }

    public class WeightService : BaseService<Weight>, IWeightService, ITransientService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public WeightService(AppDbContext context, IMapper mapper) : base(context)
        {
            _context = context;
            _mapper = mapper;
        }
    }
}