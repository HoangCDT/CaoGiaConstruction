using Microsoft.AspNetCore.Mvc;
using CaoGiaConstruction.WebClient.Areas.Admin.Dtos;
using CaoGiaConstruction.WebClient.AutoMapper.ViewModels;
using CaoGiaConstruction.WebClient.Services;

namespace CaoGiaConstruction.WebClient.Areas.Admin.Controllers
{
    public class ProductCategoryPropertiesController : BaseController
    {
        private readonly IProductCategoryPropertiesService _productCategoryPropertiesService;

        public ProductCategoryPropertiesController(IProductCategoryPropertiesService productCategoryPropertiesService)
        {
            _productCategoryPropertiesService = productCategoryPropertiesService;
        }

        [Route("/{area}/product-category-properties", Name = "admin-product-cat-pro")]
        public async Task<IActionResult> Index(SearchProductCatKeywordPagination model)
        {
            model.PageSize = 5;
            ViewBag.ParamSearch = model;

            var productCategoriesProperties = await _productCategoryPropertiesService.GetPaginationAsync(model);
            return View(productCategoriesProperties);
        }

        [Route("/{area}/product-category-properties/category/{id}")]
        public async Task<IActionResult> PropertiesByCatId(Guid catId)
        {
            var productCategoriesProperties = await _productCategoryPropertiesService.GetAllAsync(x => x.ProductCategoryId == catId);
            return Json(productCategoriesProperties);
        }

    }
}