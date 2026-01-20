using AutoMapper;
using CaoGiaConstruction.WebClient.Context;
using CaoGiaConstruction.WebClient.Context.Entities;
using CaoGiaConstruction.WebClient.Installers;

namespace CaoGiaConstruction.WebClient.Services
{
    public interface IPropertiesService : IBaseService<Properties>
    {
        bool ValidateCode(string code, Guid? id);
    }

    public class PropertiesService : BaseService<Properties>, IPropertiesService, ITransientService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public PropertiesService(AppDbContext context, IMapper mapper) : base(context)
        {
            _context = context;
            _mapper = mapper;
        }

        public bool ValidateCode(string code, Guid? id)
        {
            var currentProp = _context.Properties.FirstOrDefault(p => p.Code == code);
            if (currentProp == null) // Chưa có cái nào
                return true;
            else
            {
                if (currentProp.Id != id)
                    return false; // đã có rồi
                else return true; // Trường hợp update chính nó
            }
        }
    }
}