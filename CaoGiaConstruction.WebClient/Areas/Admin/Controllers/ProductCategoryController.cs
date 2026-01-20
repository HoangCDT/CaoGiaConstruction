using Microsoft.AspNetCore.Mvc;
using CaoGiaConstruction.WebClient.Areas.Admin.Dtos;
using CaoGiaConstruction.WebClient.AutoMapper.ViewModels;
using CaoGiaConstruction.WebClient.Services;

namespace CaoGiaConstruction.WebClient.Areas.Admin.Controllers
{
    public class ProductCategoryController : BaseController
    {
        private readonly IProductCategoryService _productCategoryService;
        private readonly IProductMainCategoryService _productMainCategoryService;

        public ProductCategoryController(IProductCategoryService productCategoryService, IProductMainCategoryService productMainCategoryService)
        {
            _productCategoryService = productCategoryService;
            _productMainCategoryService = productMainCategoryService;
        }

        [Route("/{area}/product-category", Name = "admin-product-cat")]
        public async Task<IActionResult> Index(SearchProductCatKeywordPagination model)
        {
            model.PageSize = 5;
            ViewBag.ParamSearch = model;

            ViewBag.ProductMainCategory = await _productMainCategoryService.GetAllAsync();
            var productCategories = await _productCategoryService.GetPaginationAsync(model);
            return View(productCategories);
        }

        [Route("/{area}/product-category/action", Name = "admin-product-cat-action-add")]
        [Route("/{area}/product-category/{id}/action", Name = "admin-product-cat-action")]
        public IActionResult Action()
        {
            return View();
        }

        [HttpPut]
        [Route("/{area}/product-category/{id}/status")]
        public async Task<JsonResult> UpdateStatus(Guid id)
        {
            var result = await _productCategoryService.ChangeStatus(id);
            return Json(result);
        }

        [HttpDelete]
        [Route("/{area}/product-category/{id}/delete")]
        public async Task<JsonResult> Delete(Guid id)
        {
            var result = await _productCategoryService.RemoveAsync(id);
            return Json(result);
        }

        [HttpPost]
        [Route("/{area}/product-category/addorupdate")]
        public async Task<JsonResult> AddOrUpdate([FromForm] ProductCategoryActionVM model)
        {
            var result = await _productCategoryService.AddOrUpdateActionAsync(model);
            return Json(result);
        }

        [HttpGet]
        [Route("/{area}/product-category/{id}")]
        public async Task<JsonResult> FindById(Guid id)
        {
            var result = await _productCategoryService.FindByIdAsync(id);
            return Json(result);
        }
    }
}