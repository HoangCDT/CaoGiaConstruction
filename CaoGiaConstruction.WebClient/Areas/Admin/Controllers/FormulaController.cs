using Microsoft.AspNetCore.Mvc;
using CaoGiaConstruction.WebClient.Areas.Admin.Dtos;
using CaoGiaConstruction.WebClient.AutoMapper.ViewModels;
using CaoGiaConstruction.WebClient.Context.Enums;
using CaoGiaConstruction.WebClient.Context.Entities;
using CaoGiaConstruction.WebClient.Services;

namespace CaoGiaConstruction.WebClient.Areas.Admin.Controllers
{
    public class FormulaController : BaseController
    {
        private readonly IBlogService _blogService;
        private readonly IBlogCategoryService _blogCategoryService;

        public FormulaController(IBlogService blogService, IBlogCategoryService blogCategoryService)
        {
            _blogService = blogService;
            _blogCategoryService = blogCategoryService;
        }

        [Route("/{area}/formula", Name = "admin-formula")]
        public async Task<IActionResult> Index(SearchBlogCatKeywordPagination model)
        {
            model.PageSize = 5;
            ViewBag.ParamSearch = model;
            model.Type = BlogTypeEnum.FORMULA;

            var blogCategories = await _blogService.GetPaginationAsync(model);

            //Categories
            ViewBag.FormulaCategories = await _blogCategoryService.GetAllAsync(x => x.Type == BlogTypeEnum.FORMULA);
            return View(blogCategories);
        }

        [Route("/{area}/formula/action", Name = "admin-formula-action-add")]
        [Route("/{area}/formula/{id}/action", Name = "admin-formula-action")]
        public async Task<IActionResult> Action(Guid id, string type)
        {
            //Categories
            ViewBag.FormulaCategories = await _blogCategoryService.GetAllAsync(x => x.Type == BlogTypeEnum.FORMULA);

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
        [Route("/{area}/formula/{id}/status")]
        public async Task<JsonResult> UpdateStatus(Guid id)
        {
            var result = await _blogService.ChangeStatus(id);
            return Json(result);
        }

        [HttpDelete]
        [Route("/{area}/formula/{id}/delete")]
        public async Task<JsonResult> Delete(Guid id)
        {
            var result = await _blogService.RemoveAsync(id);
            return Json(result);
        }

        [HttpPost]
        [Route("/{area}/formula/addorupdate")]
        public async Task<JsonResult> AddOrUpdate([FromForm] BlogActionVM model)
        {
            model.Type = BlogTypeEnum.FORMULA;
            var result = await _blogService.AddOrUpdateActionAsync(model);
            return Json(result);
        }

        [HttpGet]
        [Route("/{area}/formula/{id}")]
        public async Task<JsonResult> FindById(Guid id)
        {
            var result = await _blogService.FindByIdAsync(id);
            return Json(result);
        }
    }
}