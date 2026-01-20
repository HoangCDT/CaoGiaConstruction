using Microsoft.AspNetCore.Mvc;
using CaoGiaConstruction.WebClient.Areas.Admin.Dtos;
using CaoGiaConstruction.WebClient.AutoMapper.ViewModels;
using CaoGiaConstruction.WebClient.Services;

namespace CaoGiaConstruction.WebClient.Areas.Admin.Controllers
{
    public class ProductMainCategoryController : BaseController
    {
        private readonly IProductCategoryService _service;

        public ProductMainCategoryController(IProductCategoryService service)
        {
            _service = service;
        }

        [Route("/{area}/product-main-category", Name = "admin-product-main")]
        public async Task<IActionResult> Index(SearchKeywordPagination model)
        {
            model.PageSize = 5;
            ViewBag.Keyword = model.Keyword;
            var productCategories = await _service.GetPaginationAsync(model);
            return View(productCategories);
        }

        [Route("/{area}/product-main-category/action", Name = "admin-product-main-action-add")]
        [Route("/{area}/product-main-category/{id}/action", Name = "admin-product-main-action")]
        public IActionResult Action()
        {
            return View();
        }

        [HttpPut]
        [Route("/{area}/product-main-category/{id}/status")]
        public async Task<JsonResult> UpdateStatus(Guid id)
        {
            var result = await _service.ChangeStatus(id);
            return Json(result);
        }

        [HttpDelete]
        [Route("/{area}/product-main-category/{id}/delete")]
        public async Task<JsonResult> Delete(Guid id)
        {
            var result = await _service.RemoveAsync(id);
            return Json(result);
        }

        [HttpPost]
        [Route("/{area}/product-main-category/addorupdate")]
        public async Task<JsonResult> AddOrUpdate([FromForm] ProductCategoryActionVM model)
        {
            var result = await _service.AddOrUpdateActionAsync(model);
            return Json(result);
        }

        [HttpGet]
        [Route("/{area}/product-main-category/{id}")]
        public async Task<JsonResult> FindById(Guid id)
        {
            var result = await _service.FindByIdAsync(id);
            return Json(result);
        }
    }
}