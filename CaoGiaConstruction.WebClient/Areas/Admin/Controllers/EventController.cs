using Microsoft.AspNetCore.Mvc;
using CaoGiaConstruction.WebClient.Areas.Admin.Dtos;
using CaoGiaConstruction.WebClient.AutoMapper.ViewModels;
using CaoGiaConstruction.WebClient.Context.Enums;
using CaoGiaConstruction.WebClient.Context.Entities;
using CaoGiaConstruction.WebClient.Services;

namespace CaoGiaConstruction.WebClient.Areas.Admin.Controllers
{
    public class EventController : BaseController
    {
        private readonly IBlogService _blogService;
        private readonly IBlogCategoryService _blogCategoryService;

        public EventController(IBlogService blogService, IBlogCategoryService blogCategoryService)
        {
            _blogService = blogService;
            _blogCategoryService = blogCategoryService;
        }

        [Route("/{area}/event", Name = "admin-event")]
        public async Task<IActionResult> Index(SearchBlogCatKeywordPagination model)
        {
            model.PageSize = 5;
            ViewBag.ParamSearch = model;
            model.Type = BlogTypeEnum.NEWSEVENT;

            var blogCategories = await _blogService.GetPaginationAsync(model);

            //Categories
            ViewBag.EventCategories = await _blogCategoryService.GetAllAsync(x => x.Type == BlogTypeEnum.NEWSEVENT);
            return View(blogCategories);
        }

        [Route("/{area}/event/action", Name = "admin-event-action-add")]
        [Route("/{area}/event/{id}/action", Name = "admin-event-action")]
        public async Task<IActionResult> Action(Guid id, string type)
        {
            //Categories
            ViewBag.EventCategories = await _blogCategoryService.GetAllAsync(x => x.Type == BlogTypeEnum.NEWSEVENT);

            if (id != Guid.Empty)
            {
                var data = await _blogService.FindByIdAsync(id);

                if (type == "copy")
                {
                    data.Id = Guid.Empty;
                    data.Avatar = null;
                }

                return View(data);
            }
            return View(new Blog());
        }

        [HttpPut]
        [Route("/{area}/event/{id}/status")]
        public async Task<JsonResult> UpdateStatus(Guid id)
        {
            var result = await _blogService.ChangeStatus(id);
            return Json(result);
        }

        [HttpDelete]
        [Route("/{area}/event/{id}/delete")]
        public async Task<JsonResult> Delete(Guid id)
        {
            var result = await _blogService.RemoveAsync(id);
            return Json(result);
        }

        [HttpPost]
        [Route("/{area}/event/addorupdate")]
        public async Task<JsonResult> AddOrUpdate([FromForm] BlogActionVM model)
        {
            model.Type = BlogTypeEnum.NEWSEVENT;
            var result = await _blogService.AddOrUpdateActionAsync(model);
            return Json(result);
        }

        [HttpGet]
        [Route("/{area}/event/{id}")]
        public async Task<JsonResult> FindById(Guid id)
        {
            var result = await _blogService.FindByIdAsync(id);
            return Json(result);
        }
    }
}