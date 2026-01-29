using Microsoft.AspNetCore.Mvc;
using CaoGiaConstruction.WebClient.Areas.Admin.Dtos;
using CaoGiaConstruction.WebClient.AutoMapper.ViewModels;
using CaoGiaConstruction.WebClient.Context.Entities;
using CaoGiaConstruction.WebClient.Services;

namespace CaoGiaConstruction.WebClient.Areas.Admin.Controllers
{
    public class ProcessStepController : BaseController
    {
        private readonly IProcessStepService _processStepService;

        public ProcessStepController(IProcessStepService processStepService)
        {
            _processStepService = processStepService;
        }

        [Route("/{area}/process-step", Name = "admin-process-step")]
        public async Task<IActionResult> Index(SearchKeywordPagination model)
        {
            model.PageSize = 5;
            ViewBag.Keyword = model.Keyword;
            var processSteps = await _processStepService.GetPaginationAsync(model);
            return View(processSteps);
        }


        [HttpPut]
        [Route("/{area}/process-step/{id}/status")]
        public async Task<JsonResult> UpdateStatus(Guid id)
        {
            var result = await _processStepService.ChangeStatus(id);
            return Json(result);
        }

        [HttpDelete]
        [Route("/{area}/process-step/{id}/delete")]
        public async Task<JsonResult> Delete(Guid id)
        {
            var result = await _processStepService.RemoveAsync(id);
            return Json(result);
        }

        [HttpPost]
        [Route("/{area}/process-step/addorupdate")]
        public async Task<JsonResult> AddOrUpdate([FromForm] ProcessStepActionVM model)
        {
            var result = await _processStepService.AddOrUpdateActionAsync(model);
            return Json(result);
        }

        [HttpGet]
        [Route("/{area}/process-step/{id}")]
        public async Task<JsonResult> FindById(Guid id)
        {
            var result = await _processStepService.FindByIdAsync(id);
            return Json(result);
        }

        [HttpPost]
        [Route("/{area}/process-step/sort")]
        public async Task<JsonResult> UpdateSort([FromBody] List<ProcessStepSortDto> items)
        {
            var result = await _processStepService.UpdateSortOrderAsync(items);
            return Json(result);
        }
    }
}
