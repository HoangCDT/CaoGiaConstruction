using Microsoft.AspNetCore.Mvc;
using CaoGiaConstruction.WebClient.Areas.Admin.Dtos;
using CaoGiaConstruction.WebClient.Context.Entities;
using CaoGiaConstruction.WebClient.Services;

namespace CaoGiaConstruction.WebClient.Areas.Admin.Controllers
{
    public class SettingController : BaseController
    {
        private readonly ISettingService _service;

        public SettingController(ISettingService service)
        {
            _service = service;
        }

        [Route("/{area}/setting", Name = "admin-setting")]
        public async Task<IActionResult> Index(SearchKeywordPagination model)
        {
            model.PageSize = 5;
            ViewBag.Keyword = model.Keyword;
            var blogCategories = await _service.GetPaginationAsync(model);
            return View(blogCategories);
        }

        [Route("/{area}/setting/action", Name = "admin-setting-action-add")]
        [Route("/{area}/setting/{id}/action", Name = "admin-setting-action")]
        public IActionResult Action()
        {
            return View();
        }

        [HttpPut]
        [Route("/{area}/setting/{id}/status")]
        public async Task<JsonResult> UpdateStatus(Guid id)
        {
            var result = await _service.ChangeStatus(id);
            return Json(result);
        }

        [HttpDelete]
        [Route("/{area}/setting/{id}/delete")]
        public async Task<JsonResult> Delete(Guid id)
        {
            var result = await _service.RemoveAsync(id);
            return Json(result);
        }

        [HttpPost]
        [Route("/{area}/setting/addorupdate")]
        public async Task<JsonResult> AddOrUpdate([FromBody] Setting model)
        {
            var result = await _service.AddOrUpdateAsync(model);
            return Json(result);
        }

        [HttpGet]
        [Route("/{area}/setting/{id}")]
        public async Task<JsonResult> FindById(Guid id)
        {
            var result = await _service.FindByIdAsync(id);
            return Json(result);
        }
    }
}