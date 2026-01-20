using Microsoft.AspNetCore.Mvc;
using CaoGiaConstruction.WebClient.Areas.Admin.Dtos;
using CaoGiaConstruction.WebClient.AutoMapper.ViewModels;
using CaoGiaConstruction.WebClient.Services;

namespace CaoGiaConstruction.WebClient.Areas.Admin.Controllers
{
    public class BlogCategoryController : BaseController
    {
        private readonly IBlogCategoryService _blogCategoryService;

        public BlogCategoryController(IBlogCategoryService blogCategoryService)
        {
            _blogCategoryService = blogCategoryService;
        }

        [Route("/{area}/blog-category", Name = "admin-blog-cat")]
        public async Task<IActionResult> Index(SearchKeywordPagination model)
        {
            model.PageSize = 5;
            ViewBag.Keyword = model.Keyword;
            var blogCategories = await _blogCategoryService.GetPaginationAsync(model);
            return View(blogCategories);
        }

        [Route("/{area}/blog-category/action", Name = "admin-blog-cat-action-add")]
        [Route("/{area}/blog-category/{id}/action", Name = "admin-blog-cat-action")]
        public IActionResult Action()
        {
            return View();
        }

        [HttpPut]
        [Route("/{area}/blog-category/{id}/status")]
        public async Task<JsonResult> UpdateStatus(Guid id)
        {
            var result = await _blogCategoryService.ChangeStatus(id);
            return Json(result);
        }

        [HttpDelete]
        [Route("/{area}/blog-category/{id}/delete")]
        public async Task<JsonResult> Delete(Guid id)
        {
            var result = await _blogCategoryService.RemoveAsync(id);
            return Json(result);
        }

        [HttpPost]
        [Route("/{area}/blog-category/addorupdate")]
        public async Task<JsonResult> AddOrUpdate([FromForm] BlogCategoryActionVM model)
        {
            var result = await _blogCategoryService.AddOrUpdateActionAsync(model);
            return Json(result);
        }

        [HttpGet]
        [Route("/{area}/blog-category/{id}")]
        public async Task<JsonResult> FindById(Guid id)
        {
            var result = await _blogCategoryService.FindByIdAsync(id);
            return Json(result);
        }
    }
}