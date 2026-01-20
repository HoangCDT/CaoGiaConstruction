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
                             ? "Khám phá các sản phẩm cà phê, trà và thức uống sáng tạo tại Cao Gia Construction."
                             : $"Khám phá sản phẩm {model.Keyword} tại Cao Gia Construction, nơi chuyên cung cấp cà phê chất lượng và thức uống sáng tạo.", // Description
               imageUrl: null, // Logo (Ảnh đại diện trang web)
               keywords: "Cao Gia Construction, sản phẩm cà phê, trà, thức uống sáng tạo, cà phê chất lượng, trà ngon", // Keywords,
               updateTime:  null, // UpdateTime
               tag: "Cao Gia Construction, sản phẩm cà phê, trà, thức uống sáng tạo" // Tag (Các thẻ liên quan)
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
            ? "Khám phá các máy pha cà phê chất lượng cao tại Cao Gia Construction. Chúng tôi cung cấp các loại máy pha cà phê chuyên nghiệp phục vụ mọi nhu cầu."
            : $"Khám phá máy pha {model.Keyword} tại Cao Gia Construction, nơi cung cấp các sản phẩm máy pha cà phê uy tín và chất lượng.", // Description
            imageUrl: null, // Logo (Ảnh đại diện trang web)
            keywords: "Máy pha cà phê, máy móc dụng cụ, máy pha chuyên nghiệp, máy pha Cao Gia Construction, sản phẩm máy pha", // Keywords,
            updateTime: null, // UpdateTime
            tag: "Máy pha cà phê, máy móc dụng cụ, máy pha chuyên nghiệp, Cao Gia Construction" // Tag (Các thẻ liên quan)
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
               title: string.IsNullOrEmpty(model.Keyword) ? "Máy xay cà phê của Cao Gia Construction" : $"{model.Keyword} - Cao Gia Construction", // Title (Thêm tiêu đề trang chứa từ khóa chính)
               siteName: "Cao Gia Construction", // SiteName (Tên trang web hoặc công ty)
               pageType: "product", // PageType (Loại trang: product, article)
               description: string.IsNullOrEmpty(model.Keyword)
            ? "Khám phá các dòng máy xay cà phê chất lượng cao tại Cao Gia Construction. Sản phẩm đa dạng, phù hợp với cả nhu cầu gia đình và chuyên nghiệp. Mua ngay để trải nghiệm sự khác biệt!"
            : $"Khám phá máy xay {model.Keyword} tại Cao Gia Construction,thiết kế hiện đại, xay mịn chuẩn vị. Cao Gia Construction cam kết sản phẩm uy tín và dịch vụ hàng đầu", // Description
            imageUrl: null, // Logo (Ảnh đại diện trang web)
            keywords: "Máy xay cà phê, máy xay cà phê chuyên nghiệp, máy xay cafe gia đình, máy xay hạt cafe, Cao Gia Construction, máy pha cà phê, dụng cụ pha chế", // Keywords,
            updateTime: null, // UpdateTime
            tag: "Máy xay cà phê, máy xay cà phê chuyên nghiệp, máy xay cafe gia đình, máy xay hạt cafe, Cao Gia Construction, máy pha cà phê, dụng cụ pha chế" // Tag (Các thẻ liên quan)
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
                keywords: product.SeoKeywords ?? "Sản phẩm Cao Gia Construction, máy pha cà phê, sản phẩm cà phê, trà, thức uống sáng tạo", // Dynamic keywords
                updateTime: product.ModifiedDate?.ToString("yyyy-MM-ddTHH:mm:ssZ"), // Current update time in ISO format
                product.SeoKeywords ?? "Sản phẩm Cao Gia Construction, máy pha cà phê, sản phẩm cà phê" // Tags related to the product
            );

            // Set meta tags for the product detail page
            ViewBag.Header = SetMetaTags(metaTag);

            #endregion

            ViewBag.ProductRelateds = await _productService.GetProductRelatedsync(product.ProductCategoryId.ToGuid(), product.Id.ToGuid());

            return View(product);
        }
    }
}