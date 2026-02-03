using Microsoft.AspNetCore.Mvc;
using CaoGiaConstruction.WebClient.Dtos;
using CaoGiaConstruction.WebClient.Services;

namespace CaoGiaConstruction.WebClient.Controllers.ViewComponents
{
    public class vcProject : ViewComponent
    {
        private readonly IProjectService _projectService;

        public vcProject(IProjectService projectService)
        {
            _projectService = projectService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var services = await _projectService.GetServicesWithProjectsAsync();
            var projects = await _projectService.GetProjectsForHomeAsync();
            
            var model = new ProjectHomeDto
            {
                Services = services,
                Projects = projects
            };
            
            return View(model);
        }
    }
}
