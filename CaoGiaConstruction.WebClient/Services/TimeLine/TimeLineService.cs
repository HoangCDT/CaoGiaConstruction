using AutoMapper;
using CaoGiaConstruction.WebClient.Context;
using CaoGiaConstruction.WebClient.Context.Entities;
using CaoGiaConstruction.WebClient.Installers;

namespace CaoGiaConstruction.WebClient.Services
{
    public interface ITimeLineService : IBaseService<TimeLine>
    {
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
    }
}