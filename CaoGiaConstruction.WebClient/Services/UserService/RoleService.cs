using AutoMapper;
using Microsoft.AspNetCore.Identity;
using CaoGiaConstruction.Utilities.Dtos;
using CaoGiaConstruction.WebClient.Context;
using CaoGiaConstruction.WebClient.Context.Entities;
using CaoGiaConstruction.WebClient.Installers;

namespace CaoGiaConstruction.WebClient.Services
{
    public interface IRoleService
    {
    }

    public class RoleService : IRoleService, ITransientService
    {
        private readonly RoleManager<Role> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;
        private readonly MapperConfiguration _configMapper;
        private OperationResult _operationResult;

        public RoleService(RoleManager<Role> roleManager, UserManager<User> userManager,
            IMapper mapper, AppDbContext dbContext,
            MapperConfiguration configMapper)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _mapper = mapper;
            _context = dbContext;
            _configMapper = configMapper;
        }
    }
}