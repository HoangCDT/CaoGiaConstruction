using Microsoft.AspNetCore.Mvc;
using CaoGiaConstruction.WebClient.Areas.Admin.Dtos;
using CaoGiaConstruction.WebClient.AutoMapper.ViewModels;
using CaoGiaConstruction.WebClient.Services;

namespace CaoGiaConstruction.WebClient.Areas.Admin.Controllers
{
    public class ServiceCategoryController : BaseController
    {
        private readonly IServiceCategoryService _productCategoryService;

        public ServiceCategoryController(IServiceCategoryService productCategoryService)
        {
            _productCategoryService = productCategoryService;
        }

        [Route("/{area}/service-category", Name = "admin-service-cat")]
        public async Task<IActionResult> Index(SearchKeywordPagination model)
        {
            model.PageSize = 5;
            ViewBag.Keyword = model.Keyword;
            var productCategories = await _productCategoryService.GetPaginationAsync(model);
            return View(productCategories);
        }

        [Route("/{area}/service-category/action", Name = "admin-service-cat-action-add")]
        [Route("/{area}/service-category/{id}/action", Name = "admin-service-cat-action")]
        public IActionResult Action()
        {
            return View();
        }

        [HttpPut]
        [Route("/{area}/service-category/{id}/status")]
        public async Task<JsonResult> UpdateStatus(Guid id)
        {
            var result = await _productCategoryService.ChangeStatus(id);
            return Json(result);
        }

        [HttpDelete]
        [Route("/{area}/service-category/{id}/delete")]
        public async Task<JsonResult> Delete(Guid id)
        {
            var result = await _productCategoryService.RemoveAsync(id);
            return Json(result);
        }

        [HttpPost]
        [Route("/{area}/service-category/addorupdate")]
        public async Task<JsonResult> AddOrUpdate([FromForm] ServiceCategoryActionVM model)
        {
            var result = await _productCategoryService.AddOrUpdateActionAsync(model);
            return Json(result);
        }

        [HttpGet]
        [Route("/{area}/service-category/{id}")]
        public async Task<JsonResult> FindById(Guid id)
        {
            var result = await _productCategoryService.FindByIdAsync(id);
            return Json(result);
        }

        [HttpPost]
        [Route("/{area}/service-category/sort")]
        public async Task<JsonResult> UpdateSort([FromBody] List<ServiceCategorySortDto> items)
        {
            var result = await _productCategoryService.UpdateSortOrderAsync(items);
            return Json(result);
        }
    }
}