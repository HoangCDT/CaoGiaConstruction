using Microsoft.AspNetCore.Mvc;
using CaoGiaConstruction.WebClient.Dtos;
using CaoGiaConstruction.WebClient.Services;

namespace CaoGiaConstruction.WebClient.Controllers.ViewComponents
{
    public class vcService : ViewComponent
    {
        private readonly IServiceCaoGiaService _service;
        private readonly IProjectService _projectService;

        public vcService(IServiceCaoGiaService service, IProjectService projectService)
        {
            _service = service;
            _projectService = projectService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var serices = await _service.GetTopServiceHomeAsync();

            var projects = await _projectService.GetHotProjectsAsync(1);

            var result = new ServiceHomeDto
            {
                Services = serices,
                Projects = projects.FirstOrDefault(),
            };

            return View(result);
        }
    }
}