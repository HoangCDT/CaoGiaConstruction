using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using CaoGiaConstruction.WebClient.Areas.Admin.Dtos;
using CaoGiaConstruction.WebClient.AutoMapper.ViewModels;
using CaoGiaConstruction.WebClient.Context.Entities;
using CaoGiaConstruction.WebClient.Services;

namespace CaoGiaConstruction.WebClient.Areas.Admin.Controllers
{
    public class ProductController : BaseController
    {
        private readonly IProductService _productService;
        private readonly IProductCategoryService _productCategoryService;
        private readonly IProductMainCategoryService _productMainCategoryService;
        private readonly IProductCategoryPropertiesService _productCategoryPropertiesService;
        private readonly IProductPropertiesService _productPropertiesService;
        private readonly IMapper _mapper;

        public ProductController(
        IProductService productService,
        IProductCategoryService productCategoryService,
        IProductMainCategoryService productMainCategoryService,
        IProductCategoryPropertiesService productCategoryPropertiesService,
        IProductPropertiesService productPropertiesService
,
        IMapper mapper)
        {
            _productService = productService;
            _productCategoryService = productCategoryService;
            _productMainCategoryService = productMainCategoryService;
            _productCategoryPropertiesService = productCategoryPropertiesService;
            _productPropertiesService = productPropertiesService;
            _mapper = mapper;
        }

        [Route("/{area}/product", Name = "admin-product")]
        public async Task<IActionResult> Index(SearchProductCatKeywordPagination model)
        {
            model.PageSize = 5;
            ViewBag.ParamSearch = model;

            var products = await _productService.GetPaginationAsync(model);

            //Categories
            ViewBag.ProductMainCategory = await _productMainCategoryService.GetAllAsync();

            if (model.ProductMainCategoryId != Guid.Empty && model.ProductMainCategoryId != null)
                ViewBag.ProductCategories = await _productCategoryService.GetAllAsync(x => x.ProductMainCategoryId ==
                model.ProductMainCategoryId && x.Status == Context.Enums.StatusEnum.Active);
            else
                ViewBag.ProductCategories = await _productCategoryService.GetAllAsync();
            return View(products);
        }

        [Route("/{area}/product/action", Name = "admin-product-action-add")]
        [Route("/{area}/product/{id}/{type}/action", Name = "admin-product-action")]
        public async Task<IActionResult> Action(Guid id, string type)
        {
            //Categories
            ViewBag.ProductCategories = await _productCategoryService.GetAllAsync();

            if (id != Guid.Empty)
            {
                var data = await _productService.FindByIdAsync(id);
                if (type == "copy")
                {
                    data.Id = Guid.Empty;
                    data.Avatar = null;
                    data.ImageList = null;
                    if (data.ProductPrices.Any())
                    {
                        foreach (var item in data.ProductPrices)
                        {
                            item.Id = Guid.Empty;
                            item.ProductId = Guid.Empty;
                        }
                    }
                    if (data.ProductProperties.Any())
                    {
                        foreach (var item in data.ProductProperties)
                        {
                            item.Id = Guid.Empty;
                            item.ProductId = Guid.Empty;
                        }
                    }
                }
                return View(data);
            }
            return View(new Product());
        }

        [HttpPut]
        [Route("/{area}/product/{id}/status")]
        public async Task<JsonResult> UpdateStatus(Guid id)
        {
            var result = await _productService.ChangeStatus(id);
            return Json(result);
        }

        [HttpPut]
        [Route("/{area}/product/{id}/sort-images")]
        public async Task<JsonResult> SortImages(Guid id, string imageList)
        {
            var result = await _productService.SortImageListAsync(id, imageList);
            return Json(result);
        }

        [HttpDelete]
        [Route("/{area}/product/{id}/delete")]
        public async Task<JsonResult> Delete(Guid id)
        {
            var result = await _productService.RemoveAsync(id);
            return Json(result);
        }

        [HttpDelete]
        [Route("/{area}/product/{id}/delete-image")]
        public async Task<JsonResult> DeleteImage(Guid id, string path)
        {
            var result = await _productService.RemoveImageProductAsync(id, path);
            return Json(result);
        }

        [HttpPost]
        [Route("/{area}/product/addorupdate")]
        public async Task<JsonResult> AddOrUpdate([FromForm] ProductActionVM model)
        {
            var result = await _productService.AddOrUpdateActionAsync(model);
            return Json(result);
        }

        [HttpGet]
        [Route("/{area}/product/{id}")]
        public async Task<JsonResult> FindById(Guid id)
        {
            var result = await _productService.FindByIdAsync(id);
            return Json(result);
        }

        [HttpGet]
        [Route("/{area}/product/properties/{catId}/{id}")]
        public async Task<JsonResult> ProductProperties(Guid catId, Guid? id)
        {
            if (id != null && id != Guid.Empty)
            {
                var result = await _productPropertiesService.GetAllAsync(x => x.ProductId == id);
                if (result.Count() == 0 || result == null)
                {
                    var catProperties = await _productCategoryPropertiesService.GetAllAsync(x => x.ProductCategoryId == catId);
                    var resultPro = _mapper.Map<List<ProductCategoryProperties>, List<ProductProperties>>(catProperties);
                    var view = await this.RenderViewAsync("_ActionPropertiesTab", resultPro, true);
                    return Json(view);
                }
                var partialView = await this.RenderViewAsync("_ActionPropertiesTab", result, true);
                return Json(partialView);

            }
            else
            {
                var catProperties = await _productCategoryPropertiesService.GetAllAsync(x => x.ProductCategoryId == catId);
                var result = _mapper.Map<List<ProductCategoryProperties>, List<ProductProperties>>(catProperties);
                var view = await this.RenderViewAsync("_ActionPropertiesTab", result, true);
                return Json(view);

            }
        }

    }
}