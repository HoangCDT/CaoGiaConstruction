using Microsoft.AspNetCore.Mvc;
using CaoGiaConstruction.WebClient.Areas.Admin.Dtos;
using CaoGiaConstruction.WebClient.AutoMapper.ViewModels;
using CaoGiaConstruction.WebClient.Context.Entities;
using CaoGiaConstruction.WebClient.Services;

namespace CaoGiaConstruction.WebClient.Areas.Admin.Controllers
{
    public class ServiceController : BaseController
    {
        private readonly IServiceCaoGiaService _serviceService;
        private readonly IServiceCategoryService _serviceCategoryService;

        public ServiceController(IServiceCaoGiaService serviceService, IServiceCategoryService serviceCategoryService)
        {
            _serviceService = serviceService;
            _serviceCategoryService = serviceCategoryService;
        }

        [Route("/{area}/service", Name = "admin-service")]
        public async Task<IActionResult> Index(SearchServiceCatKeywordPagination model)
        {
            model.PageSize = 5;
            ViewBag.ParamSearch = model;

            var serviceCategories = await _serviceService.GetPaginationAsync(model);

            //Categories
            ViewBag.ServiceCategories = await _serviceCategoryService.GetAllAsync();
            return View(serviceCategories);
        }

        [Route("/{area}/service/action", Name = "admin-service-action-add")]
        [Route("/{area}/service/{id}/action", Name = "admin-service-action")]
        public async Task<IActionResult> Action(Guid id, string type)
        {
            //Categories
            ViewBag.ServiceCategories = await _serviceCategoryService.GetAllAsync();

            if (id != Guid.Empty)
            {
                var data = await _serviceService.FindByIdAsync(id);
                if (type == "copy")
                {
                    data.Id = Guid.Empty;
                    data.Avatar = null;
                    data.ImageList = null;
                }
                return View(data);
            }
            return View(new Service());
        }

        [HttpPut]
        [Route("/{area}/service/{id}/status")]
        public async Task<JsonResult> UpdateStatus(Guid id)
        {
            var result = await _serviceService.ChangeStatus(id);
            return Json(result);
        }

        [HttpPut]
        [Route("/{area}/service/{id}/sort-images")]
        public async Task<JsonResult> SortImages(Guid id, string imageList)
        {
            var result = await _serviceService.SortImageListAsync(id, imageList);
            return Json(result);
        }

        [HttpDelete]
        [Route("/{area}/service/{id}/delete")]
        public async Task<JsonResult> Delete(Guid id)
        {
            var result = await _serviceService.RemoveAsync(id);
            return Json(result);
        }

        [HttpDelete]
        [Route("/{area}/service/{id}/delete-image")]
        public async Task<JsonResult> DeleteImage(Guid id, string path)
        {
            var result = await _serviceService.RemoveImageServiceAsync(id, path);
            return Json(result);
        }

        [HttpPost]
        [Route("/{area}/service/addorupdate")]
        public async Task<JsonResult> AddOrUpdate([FromForm] ServiceActionVM model)
        {
            var result = await _serviceService.AddOrUpdateActionAsync(model);
            return Json(result);
        }

        [HttpGet]
        [Route("/{area}/service/{id}")]
        public async Task<JsonResult> FindById(Guid id)
        {
            var result = await _serviceService.FindByIdAsync(id);
            return Json(result);
        }
    }
}