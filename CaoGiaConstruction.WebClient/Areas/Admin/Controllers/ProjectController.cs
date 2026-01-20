using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using CaoGiaConstruction.WebClient.Areas.Admin.Dtos;
using CaoGiaConstruction.WebClient.AutoMapper.ViewModels;
using CaoGiaConstruction.WebClient.Context.Entities;
using CaoGiaConstruction.WebClient.Services;

namespace CaoGiaConstruction.WebClient.Areas.Admin.Controllers
{
    public class ProjectController : BaseController
    {
        private readonly IProjectService _projectService;
        private readonly IServiceCaoGiaService _serviceCaoGiaService;
        public ProjectController(
        IProjectService ProjectService,
        IMapper mapper,
        IServiceCaoGiaService serviceCaoGiaService)
        {
            _projectService = ProjectService;
            _serviceCaoGiaService = serviceCaoGiaService;
        }

        [Route("/{area}/project", Name = "admin-project")]
        public async Task<IActionResult> Index(SearchKeywordPagination model)
        {
            model.PageSize = 5;
            ViewBag.ParamSearch = model;
            ViewBag.Services = await _serviceCaoGiaService.GetAllAsync(x => x.Status == Context.Enums.StatusEnum.Active);
            var Projects = await _projectService.GetPaginationAsync(model);

            return View(Projects);
        }

        [Route("/{area}/project/action", Name = "admin-project-action-add")]
        [Route("/{area}/project/{id}/action", Name = "admin-project-action")]
        public async Task<IActionResult> Action(Guid id, string type)
        {
            ViewBag.Services = await _serviceCaoGiaService.GetAllAsync(x => x.Status == Context.Enums.StatusEnum.Active);
            if (id != Guid.Empty)
            {
                var data = await _projectService.FindByIdAsync(id);
                if (type == "copy")
                {
                    data.Id = Guid.Empty;
                    data.Avatar = null;
                    data.ImageList = null;
                }
                return View(data);
            }
            return View(new Project());
        }

        [HttpPut]
        [Route("/{area}/project/{id}/status")]
        public async Task<JsonResult> UpdateStatus(Guid id)
        {
            var result = await _projectService.ChangeStatus(id);
            return Json(result);
        }

        [HttpPut]
        [Route("/{area}/project/{id}/sort-images")]
        public async Task<JsonResult> SortImages(Guid id, string imageList)
        {
            var result = await _projectService.SortImageListAsync(id, imageList);
            return Json(result);
        }

        [HttpDelete]
        [Route("/{area}/project/{id}/delete")]
        public async Task<JsonResult> Delete(Guid id)
        {
            var result = await _projectService.RemoveAsync(id);
            return Json(result);
        }

        [HttpDelete]
        [Route("/{area}/project/{id}/delete-image")]
        public async Task<JsonResult> DeleteImage(Guid id, string path)
        {
            var result = await _projectService.RemoveImageProjectAsync(id, path);
            return Json(result);
        }

        [HttpPost]
        [Route("/{area}/project/addorupdate")]
        public async Task<JsonResult> AddOrUpdate([FromForm] ProjectActionVM model)
        {
            var result = await _projectService.AddOrUpdateActionAsync(model);
            return Json(result);
        }

        [HttpGet]
        [Route("/{area}/project/{id}")]
        public async Task<JsonResult> FindById(Guid id)
        {
            var result = await _projectService.FindByIdAsync(id);
            return Json(result);
        }
    }
}