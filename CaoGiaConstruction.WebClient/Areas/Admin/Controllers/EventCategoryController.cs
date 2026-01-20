using Microsoft.AspNetCore.Mvc;
using CaoGiaConstruction.WebClient.Areas.Admin.Dtos;
using CaoGiaConstruction.WebClient.AutoMapper.ViewModels;
using CaoGiaConstruction.WebClient.Context.Enums;
using CaoGiaConstruction.WebClient.Services;

namespace CaoGiaConstruction.WebClient.Areas.Admin.Controllers
{
    public class EventCategoryController : BaseController
    {
        private readonly IBlogCategoryService _service;

        public EventCategoryController(IBlogCategoryService service)
        {
            _service = service;
        }

        [Route("/{area}/event-category", Name = "admin-event-cat")]
        public async Task<IActionResult> Index(SearchBlogCatKeywordPagination model)
        {
            model.PageSize = 5;
            model.Type = BlogTypeEnum.NEWSEVENT;
            ViewBag.Keyword = model.Keyword;

            var blogCategories = await _service.GetPaginationAsync(model);
            
            return View(blogCategories);
        }

        [Route("/{area}/event-category/action", Name = "admin-event-cat-action-add")]
        [Route("/{area}/event-category/{id}/action", Name = "admin-event-cat-action")]
        public IActionResult Action()
        {
            return View();
        }

        [HttpPut]
        [Route("/{area}/event-category/{id}/status")]
        public async Task<JsonResult> UpdateStatus(Guid id)
        {
            var result = await _service.ChangeStatus(id);
            return Json(result);
        }

        [HttpDelete]
        [Route("/{area}/event-category/{id}/delete")]
        public async Task<JsonResult> Delete(Guid id)
        {
            var result = await _service.RemoveAsync(id);
            return Json(result);
        }

        [HttpPost]
        [Route("/{area}/event-category/addorupdate")]
        public async Task<JsonResult> AddOrUpdate([FromForm] BlogCategoryActionVM model)
        {
            model.Type = BlogTypeEnum.NEWSEVENT;
            var result = await _service.AddOrUpdateActionAsync(model);
            return Json(result);
        }

        [HttpGet]
        [Route("/{area}/event-category/{id}")]
        public async Task<JsonResult> FindById(Guid id)
        {
            var result = await _service.FindByIdAsync(id);
            return Json(result);
        }
    }
}