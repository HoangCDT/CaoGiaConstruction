using Microsoft.AspNetCore.Mvc;
using CaoGiaConstruction.WebClient.Areas.Admin.Dtos;
using CaoGiaConstruction.WebClient.AutoMapper.ViewModels;
using CaoGiaConstruction.WebClient.Context.Entities;
using CaoGiaConstruction.WebClient.Services;

namespace CaoGiaConstruction.WebClient.Areas.Admin.Controllers
{
    public class CoreValueController : BaseController
    {
        private readonly ICoreValueService _service;

        public CoreValueController(ICoreValueService service)
        {
            _service = service;
        }

        [Route("/{area}/core-value", Name = "admin-core-value")]
        public async Task<IActionResult> Index(SearchKeywordPagination model)
        {
            model.PageSize = 5;
            ViewBag.Keyword = model.Keyword;
            var coreValues = await _service.GetPaginationAsync(model);
            return View(coreValues);
        }


        [HttpPut]
        [Route("/{area}/core-value/{id}/status")]
        public async Task<JsonResult> UpdateStatus(Guid id)
        {
            var result = await _service.ChangeStatus(id);
            return Json(result);
        }

        [HttpDelete]
        [Route("/{area}/core-value/{id}/delete")]
        public async Task<JsonResult> Delete(Guid id)
        {
            var result = await _service.RemoveAsync(id);
            return Json(result);
        }

        [HttpPost]
        [Route("/{area}/core-value/addorupdate")]
        public async Task<JsonResult> AddOrUpdate([FromForm] CoreValueActionVM model)
        {
            var result = await _service.AddOrUpdateActionAsync(model);
            return Json(result);
        }

        [HttpGet]
        [Route("/{area}/core-value/{id}")]
        public async Task<JsonResult> FindById(Guid id)
        {
            var result = await _service.FindByIdAsync(id);
            return Json(result);
        }

        [HttpPost]
        [Route("/{area}/core-value/sort")]
        public async Task<JsonResult> UpdateSort([FromBody] List<CoreValueSortDto> items)
        {
            var result = await _service.UpdateSortOrderAsync(items);
            return Json(result);
        }
    }
}
