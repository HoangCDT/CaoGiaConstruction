using AutoMapper;
using CaoGiaConstruction.WebClient.Context;
using CaoGiaConstruction.WebClient.Context.Entities;
using CaoGiaConstruction.WebClient.Installers;

namespace CaoGiaConstruction.WebClient.Services
{
    public interface IDefineSystemService : IBaseService<DefineSystem>
    {
    }

    public class DefineSystemService : BaseService<DefineSystem>, IDefineSystemService, ITransientService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public DefineSystemService(AppDbContext context, IMapper mapper) : base(context)
        {
            _context = context;
            _mapper = mapper;
        }
    }
}