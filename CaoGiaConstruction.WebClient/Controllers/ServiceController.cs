using Microsoft.AspNetCore.Mvc;
using CaoGiaConstruction.Utilities;
using CaoGiaConstruction.Utilities.Constants;
using CaoGiaConstruction.WebClient.Context.Entities;
using CaoGiaConstruction.WebClient.Dtos;
using CaoGiaConstruction.WebClient.Extensions;
using CaoGiaConstruction.WebClient.Services;
using static CaoGiaConstruction.Utilities.SetMetaTagUtility;

namespace CaoGiaConstruction.WebClient.Controllers
{
    public class ServiceController : BaseClientController
    {
        private readonly ILogger<ServiceController> _logger;
        private readonly IServiceCaoGiaService _service;

        public ServiceController(ILogger<ServiceController> logger, IServiceCaoGiaService service)
        {
            _logger = logger;
            _service = service;
        }

        [Route("/dich-vu", Name = "service")]
        [Route("/dich-vu/{code}", Name = "service-cate")]
        public async Task<IActionResult> Service(SearchServiceClientDto model)
        {
            model.PageSize = 12;
            ViewBag.Param = model;

            var data = await _service.GetPaginationServiceClientAsync(model);

            #region Seo Meta Tag
            var metaTag = BuildMetaTag(
                title: !string.IsNullOrEmpty(model.Keyword) ? $"{model.Keyword} - Dịch vụ tại Cao Gia Construction" : "Dịch vụ tại Cao Gia Construction", // Dynamic title based on the product title
                siteName: "Cao Gia Construction", // Site name
                pageType: "service", // Page type: product-detail (for detailed product page)
                description: "Khám phá các dịch vụ tại Cao Gia Construction, bao gồm các sản phẩm cà phê, máy pha cà phê, và thức uống sáng tạo.", // Dynamic description
                imageUrl: null, // Image URL (avatar or first image in the list)
                keywords: "Dịch vụ Cao Gia Construction, cà phê, máy pha cà phê, thức uống sáng tạo", // Dynamic keywords
                updateTime: null, // Current update time in ISO format
                tag: "Dịch vụ Cao Gia Construction, cà phê, máy pha cà phê, thức uống sáng tạo" // Tags related to the product
            );
            // Set meta tags for the product detail page
            ViewBag.Header = SetMetaTags(metaTag);
            #endregion

            return View(data);
        }

        [Route("/dich-vu/{category}/{code}", Name = "service-detail")]
        public async Task<IActionResult> Detail(string code)
        {
            var service = await _service.FindServiceByCodeAsync(code);

            if (service == null)
            {
                return RedirectToRoute("error", new { code = StatusCodes.Status404NotFound });
            }

            ViewBag.ServiceRelateds = await _service.GetServiceRelatedsync(service.ServiceCategoryId.ToGuid(), service.Id.ToGuid());

            #region Seo Meta Tag
            var metaTag = BuildMetaTag(
                title: !string.IsNullOrEmpty(service.Title) ? $"{service.Title} - Dịch vụ tại Cao Gia Construction" : "Dịch vụ tại Cao Gia Construction", // Dynamic title based on the product title
                siteName: "Cao Gia Construction", // Site name
                pageType: "service-detail", // Page type: product-detail (for detailed product page)
                description: !string.IsNullOrEmpty(service.SeoDescription) ? service.SeoDescription : "Khám phá dịch vụ tại Cao Gia Construction với các lựa chọn về cà phê, trà, và thức uống sáng tạo.", // Dynamic description
                imageUrl: service.Avatar, // Image URL (avatar or first image in the list)
                keywords: "Dịch vụ Cao Gia Construction, cà phê, máy pha cà phê, thức uống sáng tạo", // Dynamic keywords
                updateTime: null, // Current update time in ISO format
                tag: "Dịch vụ Cao Gia Construction, cà phê, máy pha cà phê, thức uống sáng tạo" // Tags related to the product
            );
            // Set meta tags for the product detail page
            ViewBag.Header = SetMetaTags(metaTag);
            #endregion

            return View(service);
        }
    }
}