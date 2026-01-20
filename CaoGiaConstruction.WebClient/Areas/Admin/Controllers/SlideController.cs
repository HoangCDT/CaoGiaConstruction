using Microsoft.AspNetCore.Mvc;
using CaoGiaConstruction.WebClient.Areas.Admin.Dtos;
using CaoGiaConstruction.WebClient.AutoMapper.ViewModels;
using CaoGiaConstruction.WebClient.Services;

namespace CaoGiaConstruction.WebClient.Areas.Admin.Controllers
{
    public class slideController : BaseController
    {
        private readonly ISlideService _slideService;

        public slideController(ISlideService slideService)
        {
            _slideService = slideService;
        }

        [Route("/{area}/slide", Name = "admin-slide")]
        public async Task<IActionResult> Index(SearchSlideKeywordPagination model)
        {
            model.PageSize = 5;
            ViewBag.ParamSearch = model;
            var productCategories = await _slideService.GetPaginationAsync(model);

            return View(productCategories);
        }

        [Route("/{area}/slide/action", Name = "admin-slide-action-add")]
        [Route("/{area}/slide/{id}/action", Name = "admin-slide-action")]
        public IActionResult Action()
        {
            return View();
        }

        [HttpPut]
        [Route("/{area}/slide/{id}/status")]
        public async Task<JsonResult> UpdateStatus(Guid id)
        {
            var result = await _slideService.ChangeStatus(id);
            return Json(result);
        }

        [HttpDelete]
        [Route("/{area}/slide/{id}/delete")]
        public async Task<JsonResult> Delete(Guid id)
        {
            var result = await _slideService.RemoveAsync(id);
            return Json(result);
        }

        [HttpPost]
        [Route("/{area}/slide/addorupdate")]
        public async Task<JsonResult> AddOrUpdate([FromForm] SlideActionVM model)
        {
            var result = await _slideService.AddOrUpdateActionAsync(model);
            return Json(result);
        }

        [HttpGet]
        [Route("/{area}/slide/{id}")]
        public async Task<JsonResult> FindById(Guid id)
        {
            var result = await _slideService.FindByIdAsync(id);
            return Json(result);
        }
    }
}