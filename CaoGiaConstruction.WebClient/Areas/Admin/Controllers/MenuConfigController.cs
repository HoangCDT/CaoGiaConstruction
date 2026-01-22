using Microsoft.AspNetCore.Mvc;
using CaoGiaConstruction.WebClient.Areas.Admin.Dtos;
using CaoGiaConstruction.WebClient.Context.Entities;
using CaoGiaConstruction.WebClient.Services;

namespace CaoGiaConstruction.WebClient.Areas.Admin.Controllers
{
    public class MenuConfigController : BaseController
    {
        private readonly IMenuConfigService _service;

        public MenuConfigController(IMenuConfigService service)
        {
            _service = service;
        }

        [Route("/{area}/menu-config", Name = "admin-menu-config")]
        public async Task<IActionResult> Index(SearchKeywordPagination model)
        {
            model.PageSize = 50;
            ViewBag.Keyword = model.Keyword;
            var configs = await _service.GetPaginationAsync(model);
            return View(configs);
        }

        [HttpPut]
        [Route("/{area}/menu-config/{id}/activate")]
        public async Task<JsonResult> Activate(Guid id)
        {
            var result = await _service.SetActiveAsync(id);
            return Json(result);
        }

        [HttpPut]
        [Route("/{area}/menu-config/{id}/deactivate")]
        public async Task<JsonResult> Deactivate(Guid id)
        {
            var result = await _service.SetInactiveAsync(id);
            return Json(result);
        }

        [HttpDelete]
        [Route("/{area}/menu-config/{id}/delete")]
        public async Task<JsonResult> Delete(Guid id)
        {
            var result = await _service.RemoveAsync(id);
            return Json(result);
        }

        [HttpPost]
        [Route("/{area}/menu-config/addorupdate")]
        public async Task<JsonResult> AddOrUpdate([FromBody] MenuConfig model)
        {
            var result = await _service.AddOrUpdateAsync(model);
            return Json(result);
        }

        [HttpGet]
        [Route("/{area}/menu-config/{id}")]
        public async Task<JsonResult> FindById(Guid id)
        {
            var result = await _service.FindByIdAsync(id);
            return Json(result);
        }
    }
}
