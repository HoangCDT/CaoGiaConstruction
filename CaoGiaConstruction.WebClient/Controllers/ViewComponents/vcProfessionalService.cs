using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CaoGiaConstruction.WebClient.Dtos;
using CaoGiaConstruction.WebClient.Services;
using CaoGiaConstruction.WebClient.Context;
using CaoGiaConstruction.WebClient.Context.Enums;
using CaoGiaConstruction.WebClient.AutoMapper.ViewModels;
using AutoMapper;

namespace CaoGiaConstruction.WebClient.Controllers.ViewComponents
{
    public class vcProfessionalService : ViewComponent
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IProjectService _projectService;
        private readonly ISettingService _settingService;

        public vcProfessionalService(AppDbContext context, IMapper mapper, IProjectService projectService, ISettingService settingService)
        {
            _context = context;
            _mapper = mapper;
            _projectService = projectService;
            _settingService = settingService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Get services ordered by SortOrder (ascending) to get the first 3 professional services
            var services = await _context.Services
                .Include(x => x.ServiceCategory)
                .Where(x => x.Status == StatusEnum.Active)
                .OrderBy(x => x.SortOrder)
                .Take(4)
                .AsNoTracking()
                .ToListAsync();

            var servicesVM = _mapper.Map<List<ServiceNoContentVM>>(services);

            var projects = await _projectService.GetHotProjectsAsync(1);
            var setting = await _settingService.GetSettingCacheAsync();

            var result = new ProfessionalServiceDto
            {
                Services = servicesVM.Skip(1).Take(3).ToList(),
                FeaturedProject = servicesVM.FirstOrDefault(),
                Setting = setting
            };

            return View(result);
        }
    }
}
