using Microsoft.AspNetCore.Mvc;
using CaoGiaConstruction.WebClient.Areas.Admin.Dtos;
using CaoGiaConstruction.WebClient.AutoMapper.ViewModels;
using CaoGiaConstruction.WebClient.Context.Enums;
using CaoGiaConstruction.WebClient.Context.Entities;
using CaoGiaConstruction.WebClient.Services;

namespace CaoGiaConstruction.WebClient.Areas.Admin.Controllers
{
    public class BlogController : BaseController
    {
        private readonly IBlogService _blogService;
        private readonly IBlogCategoryService _blogCategoryService;

        public BlogController(IBlogService blogService, IBlogCategoryService blogCategoryService)
        {
            _blogService = blogService;
            _blogCategoryService = blogCategoryService;
        }

        [Route("/{area}/blog", Name = "admin-blog")]
        public async Task<IActionResult> Index(SearchBlogCatKeywordPagination model)
        {
            model.PageSize = 5;
            model.Type = BlogTypeEnum.KNOWLEDGE;
            ViewBag.ParamSearch = model;

            var blogCategories = await _blogService.GetPaginationAsync(model);

            //Categories
            ViewBag.BlogCategories = await _blogCategoryService.GetAllAsync(x => x.Type == BlogTypeEnum.KNOWLEDGE);
            return View(blogCategories);
        }

        [Route("/{area}/blog/action", Name = "admin-blog-action-add")]
        [Route("/{area}/blog/{id}/action", Name = "admin-blog-action")]
        public async Task<IActionResult> Action(Guid id, string type)
        {
            //Categories
            ViewBag.BlogCategories = await _blogCategoryService.GetAllAsync(x => x.Type == BlogTypeEnum.KNOWLEDGE);

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
        [Route("/{area}/blog/{id}/status")]
        public async Task<JsonResult> UpdateStatus(Guid id)
        {
            var result = await _blogService.ChangeStatus(id);
            return Json(result);
        }

        [HttpDelete]
        [Route("/{area}/blog/{id}/delete")]
        public async Task<JsonResult> Delete(Guid id)
        {
            var result = await _blogService.RemoveAsync(id);
            return Json(result);
        }

        [HttpPost]
        [Route("/{area}/blog/addorupdate")]
        public async Task<JsonResult> AddOrUpdate([FromForm] BlogActionVM model)
        {
            model.Type = BlogTypeEnum.KNOWLEDGE;
            var result = await _blogService.AddOrUpdateActionAsync(model);
            return Json(result);
        }

        [HttpGet]
        [Route("/{area}/blog/{id}")]
        public async Task<JsonResult> FindById(Guid id)
        {
            var result = await _blogService.FindByIdAsync(id);
            return Json(result);
        }
    }
}