using Microsoft.AspNetCore.Mvc;
using CaoGiaConstruction.Utilities;
using CaoGiaConstruction.WebClient.Const;
using CaoGiaConstruction.WebClient.Dtos;
using CaoGiaConstruction.WebClient.Services;
using static CaoGiaConstruction.Utilities.SetMetaTagUtility;

namespace CaoGiaConstruction.WebClient.Controllers
{
    public class ProductController : BaseClientController
    {
        private readonly ILogger<ProductController> _logger;
        private readonly IProductService _productService;

        public ProductController(ILogger<ProductController> logger, IProductService productService)
        {
            _logger = logger;
            _productService = productService;
        }

        [Route("/san-pham", Name = "product")]
        [Route("/san-pham/{code}", Name = "product-cate")]
        public async Task<IActionResult> Index(SearchProductClientDto model)
        {
            model.PageSize = 2;
            ViewBag.Param = model;
            model.ProductMainCategoryCode = ProductMainCategoryCodeDefine.PRODUCT;
            var data = await _productService.GetGroupedProductsByCategoriesAsync(model);

            #region Seo Meta Tag
            var metaTag = BuildMetaTag(
               title: "Sản phẩm của Cao Gia Construction", // Title (Thêm tiêu đề trang chứa từ khóa chính)
               siteName: "Cao Gia Construction", // SiteName (Tên trang web hoặc công ty)
               pageType: "product", // PageType (Loại trang: product, article)
               description:  string.IsNullOrEmpty(model.Keyword)
                             ? "Khám phá các sản phẩm vật liệu xây dựng và thiết bị tại Cao Gia Construction."
                             : $"Khám phá sản phẩm {model.Keyword} tại Cao Gia Construction, nơi chuyên cung cấp vật liệu xây dựng chất lượng và thiết bị chuyên nghiệp.", // Description
               imageUrl: null, // Logo (Ảnh đại diện trang web)
               keywords: "Cao Gia Construction, vật liệu xây dựng, thiết bị xây dựng, sản phẩm xây dựng chất lượng cao", // Keywords,
               updateTime:  null, // UpdateTime
               tag: "Cao Gia Construction, vật liệu xây dựng, thiết bị xây dựng, sản phẩm xây dựng" // Tag (Các thẻ liên quan)
            );

            ViewBag.Header = SetMetaTags(metaTag);
            #endregion

            return View(data);
        }

        [Route("/may-moc-dung-cu", Name = "machines")]
        [Route("/may-moc-dung-cu/{code}", Name = "machines-cate")]
        public async Task<IActionResult> Machines(SearchProductClientDto model)
        {
            model.PageSize = 12;
            ViewBag.Param = model;
            model.ProductMainCategoryCode = ProductMainCategoryCodeDefine.MACHINES;
            var data = await _productService.GetGroupedProductsByCategoriesAsync(model);

            #region Seo Meta Tag
            var metaTag = BuildMetaTag(
               title: string.IsNullOrEmpty(model.Keyword) ? "Máy pha của Cao Gia Construction" : $"{model.Keyword} - Cao Gia Construction", // Title (Thêm tiêu đề trang chứa từ khóa chính)
               siteName:  "Cao Gia Construction", // SiteName (Tên trang web hoặc công ty)
               pageType: "product", // PageType (Loại trang: product, article)
               description: string.IsNullOrEmpty(model.Keyword)
            ? "Khám phá các máy móc và thiết bị xây dựng chất lượng cao tại Cao Gia Construction. Chúng tôi cung cấp các loại máy móc xây dựng chuyên nghiệp phục vụ mọi nhu cầu."
            : $"Khám phá máy móc {model.Keyword} tại Cao Gia Construction, nơi cung cấp các sản phẩm máy móc xây dựng uy tín và chất lượng.", // Description
            imageUrl: null, // Logo (Ảnh đại diện trang web)
            keywords: "Máy móc xây dựng, thiết bị xây dựng, máy móc chuyên nghiệp, máy móc Cao Gia Construction, sản phẩm máy móc", // Keywords,
            updateTime: null, // UpdateTime
            tag: "Máy móc xây dựng, thiết bị xây dựng, máy móc chuyên nghiệp, Cao Gia Construction" // Tag (Các thẻ liên quan)
            );

            ViewBag.Header = SetMetaTags(metaTag);
            #endregion

            return View(data);
        }


        [Route("/may-xay-ca-phe", Name = "grinder")]
        [Route("/may-xay-ca-phe/{code}", Name = "grinder-cate")]
        public async Task<IActionResult> Grinder(SearchProductClientDto model)
        {
            model.PageSize = 12;
            ViewBag.Param = model;
            model.ProductMainCategoryCode = ProductMainCategoryCodeDefine.GRINDER;
            var data = await _productService.GetGroupedProductsByCategoriesAsync(model);

            #region Seo Meta Tag
            var metaTag = BuildMetaTag(
               title: string.IsNullOrEmpty(model.Keyword) ? "Máy móc thiết bị của Cao Gia Construction" : $"{model.Keyword} - Cao Gia Construction", // Title (Thêm tiêu đề trang chứa từ khóa chính)
               siteName: "Cao Gia Construction", // SiteName (Tên trang web hoặc công ty)
               pageType: "product", // PageType (Loại trang: product, article)
               description: string.IsNullOrEmpty(model.Keyword)
            ? "Khám phá các dòng máy móc và thiết bị xây dựng chất lượng cao tại Cao Gia Construction. Sản phẩm đa dạng, phù hợp với cả nhu cầu dân dụng và công nghiệp. Liên hệ ngay để được tư vấn!"
            : $"Khám phá máy móc {model.Keyword} tại Cao Gia Construction, thiết kế hiện đại, chất lượng cao. Cao Gia Construction cam kết sản phẩm uy tín và dịch vụ hàng đầu", // Description
            imageUrl: null, // Logo (Ảnh đại diện trang web)
            keywords: "Máy móc xây dựng, thiết bị xây dựng, máy móc chuyên nghiệp, máy móc công nghiệp, Cao Gia Construction, thiết bị thi công, dụng cụ xây dựng", // Keywords,
            updateTime: null, // UpdateTime
            tag: "Máy móc xây dựng, thiết bị xây dựng, máy móc chuyên nghiệp, máy móc công nghiệp, Cao Gia Construction, thiết bị thi công, dụng cụ xây dựng" // Tag (Các thẻ liên quan)
            );

            ViewBag.Header = SetMetaTags(metaTag);
            #endregion

            return View(data);
        }

        [Route("/{category}/{code}", Name = "product-detail")]
        public async Task<IActionResult> Detail(string code)
        {
            var product = await _productService.FindProductByCodeAsync(code);

            if (product == null)
            {
                return RedirectToRoute("error", new { code = StatusCodes.Status404NotFound });
            }

            string logo = product.Avatar == null
            ? (product.ImageList.ToHostImage())
            : product.Avatar;

            #region Seo Meta Tag
            var metaTag = BuildMetaTag(
                title: product.Title, // Dynamic title based on the product title
                siteName: "Cao Gia Construction", // Site name
                pageType: "product-detail", // Page type: product-detail (for detailed product page)
                description: product.Description ?? "Mô tả chi tiết sản phẩm từ Cao Gia Construction.", // Dynamic description
                logo, // Image URL (avatar or first image in the list)
                keywords: product.SeoKeywords ?? "Sản phẩm Cao Gia Construction, vật liệu xây dựng, thiết bị xây dựng, sản phẩm xây dựng chất lượng cao", // Dynamic keywords
                updateTime: product.ModifiedDate?.ToString("yyyy-MM-ddTHH:mm:ssZ"), // Current update time in ISO format
                product.SeoKeywords ?? "Sản phẩm Cao Gia Construction, vật liệu xây dựng, thiết bị xây dựng" // Tags related to the product
            );

            // Set meta tags for the product detail page
            ViewBag.Header = SetMetaTags(metaTag);

            #endregion

            ViewBag.ProductRelateds = await _productService.GetProductRelatedsync(product.ProductCategoryId.ToGuid(), product.Id.ToGuid());

            return View(product);
        }
    }
}