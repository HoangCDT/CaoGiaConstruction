using Microsoft.AspNetCore.Mvc;
using CaoGiaConstruction.WebClient.Areas.Admin.Dtos;
using CaoGiaConstruction.WebClient.Context.Entities;
using CaoGiaConstruction.WebClient.Services;

namespace CaoGiaConstruction.WebClient.Areas.Admin.Controllers
{
    public class HomeConfigController : BaseController
    {
        private readonly IHomeComponentConfigService _service;

        public HomeConfigController(IHomeComponentConfigService service)
        {
            _service = service;
        }

        [Route("/{area}/HomeConfig/Index", Name = "admin-homeconfig")]
        [Route("/{area}/home-config")]
        public async Task<IActionResult> Index(SearchKeywordPagination model)
        {
            model.PageSize = 50;
            ViewBag.Keyword = model.Keyword;
            var configs = await _service.GetPaginationAsync(model);
            return View(configs);
        }

        [HttpPut]
        [Route("/{area}/home-config/{id}/status")]
        public async Task<JsonResult> UpdateStatus(Guid id)
        {
            var result = await _service.ChangeStatus(id);
            return Json(result);
        }

        [HttpDelete]
        [Route("/{area}/home-config/{id}/delete")]
        public async Task<JsonResult> Delete(Guid id)
        {
            var result = await _service.RemoveAsync(id);
            return Json(result);
        }

        [HttpPost]
        [Route("/{area}/home-config/addorupdate")]
        public async Task<JsonResult> AddOrUpdate([FromBody] HomeComponentConfig model)
        {
            var result = await _service.AddOrUpdateAsync(model);
            return Json(result);
        }

        [HttpGet]
        [Route("/{area}/home-config/{id}")]
        public async Task<JsonResult> FindById(Guid id)
        {
            var result = await _service.FindByIdAsync(id);
            return Json(result);
        }

        [HttpPost]
        [Route("/{area}/home-config/sort")]
        public async Task<JsonResult> UpdateSort([FromBody] List<HomeComponentConfigSortDto> items)
        {
            var result = await _service.UpdateSortOrderAsync(items);
            return Json(result);
        }
    }
}
