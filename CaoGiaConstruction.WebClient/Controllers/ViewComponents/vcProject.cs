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

        public async Task<IViewComponentResult> InvokeAsync(int? page = null, Guid? serviceId = null, bool? partial = null)
        {
            var currentPage = page ?? 1;
            if (!page.HasValue && int.TryParse(Request.Query["page"], out var parsedPage) && parsedPage > 0)
            {
                currentPage = parsedPage;
            }

            const int pageSize = 3;
            var services = await _projectService.GetServicesWithProjectsAsync();
            var projectsPager = await _projectService.GetProjectsForHomePagedAsync(currentPage, pageSize, serviceId);

            var model = new ProjectHomeDto
            {
                Services = services,
                Projects = projectsPager.Result,
                ProjectsPager = projectsPager
            };

            if (partial == true)
            {
                return View("ProjectContent", model);
            }

            return View(model);
        }
    }
}
