using Microsoft.AspNetCore.Mvc;
using CaoGiaConstruction.Utilities;
using CaoGiaConstruction.Utilities.Constants;
using CaoGiaConstruction.WebClient.Context.Enums;
using CaoGiaConstruction.WebClient.Dtos;
using CaoGiaConstruction.WebClient.Services;
using static CaoGiaConstruction.Utilities.SetMetaTagUtility;

namespace CaoGiaConstruction.WebClient.Controllers
{
    public class BlogController : BaseClientController
    {
        private readonly ILogger<BlogController> _logger;
        private readonly IBlogService _blogService;
        private readonly IAboutService _aboutService;

        public BlogController(ILogger<BlogController> logger, IBlogService blogService, IAboutService aboutService)
        {
            _logger = logger;
            _blogService = blogService;
            _aboutService = aboutService;
        }

        [Route("/tin-tuc", Name = "blog")]
        [Route("/tin-tuc/{code}", Name = "blog-cate")]
        public async Task<IActionResult> Index(SearchBlogClientDto model)
        {
            var logo = await _aboutService.GetLogoTopCacheAsync();
            #region Seo Meta Tag
            var metaTag = BuildMetaTag(
                        title: "Tin tức mới nhất về cà phê - Cao Gia Construction", // Title (Thêm tiêu đề trang chứa từ khóa chính)
                        siteName: "Cao Gia Construction - Chuyên cà phê chất lượng cao", // SiteName (Tên trang web hoặc công ty)
                        pageType: "article", // PageType (Loại trang: product, article)
                        description: "Cập nhật các tin tức và sự kiện mới nhất về cà phê, sản phẩm sáng tạo, và những thức uống đặc biệt tại Cao Gia Construction.", // Description
                        imageUrl: logo, // Logo (Ảnh đại diện trang web)
                        keywords: "cà phê, tin tức cà phê, Cao Gia Construction, cà phê sáng tạo, đồ uống đặc biệt", // Keywords,
                        updateTime: null, // UpdateTime
                        tag: "tin tức cà phê, sự kiện cà phê, sản phẩm cà phê" // Tag (Các thẻ liên quan)
                        );

            ViewBag.Header = SetMetaTags(metaTag);
            #endregion

            return await HandleBlogRequest(model, type: BlogTypeEnum.NEWSEVENT, nameof(Index));
        }

        [Route("/kien-thuc", Name = "know")]
        [Route("/kien-thuc/{code}", Name = "know-cate")]
        public async Task<IActionResult> Know(SearchBlogClientDto model)
        {
            var logo = await _aboutService.GetLogoTopCacheAsync() ?? Commons.LOGO_TOP;
            #region Seo Meta Tag
            var metaTag = BuildMetaTag(
                        title: "Kiến thức cà phê - Tìm hiểu mọi điều về cà phê | Cao Gia Construction", // Title (Thêm tiêu đề trang chứa từ khóa chính)
                        siteName: "Cao Gia Construction", // SiteName (Tên trang web hoặc công ty)
                        pageType: "knowledge", // PageType (Loại trang: product, article)
                        description: "Khám phá mọi kiến thức về cà phê tại Cao Gia Construction. Chúng tôi chia sẻ những mẹo pha chế, sự thật thú vị về cà phê, và nhiều điều bổ ích.", // Description
                        imageUrl: logo, // Logo (Ảnh đại diện trang web)
                        keywords: "kiến thức cà phê, mẹo pha chế cà phê, cà phê ngon", // Keywords,
                        updateTime: null, // UpdateTime
                        tag: "kiến thức, cà phê, mẹo pha chế" // Tag (Các thẻ liên quan)
                        );

            ViewBag.Header = SetMetaTags(metaTag);
            #endregion

            return await HandleBlogRequest(model, type: BlogTypeEnum.KNOWLEDGE, nameof(Know));
        }

        [Route("/cong-thuc", Name = "formula")]
        [Route("/cong-thuc/{code}", Name = "formula-cate")]
        public async Task<IActionResult> Formula(SearchBlogClientDto model)
        {
            var logo = await _aboutService.GetLogoTopCacheAsync() ?? Commons.LOGO_TOP;
            #region Seo Meta Tag
            var metaTag = BuildMetaTag(
                        title: "Công Thức Pha Chế Cà Phê Ngon - Bí Quyết Độc Đáo | Cao Gia Construction", // Title (Thêm tiêu đề trang chứa từ khóa chính)
                        siteName: "Cao Gia Construction", // SiteName (Tên trang web hoặc công ty)
                        pageType: "article", // PageType (Loại trang: product, article)
                        description: "Khám phá công thức pha chế cà phê ngon và sáng tạo tại Cao Gia Construction. Cùng chúng tôi thực hiện những món đồ uống độc đáo để thỏa mãn niềm đam mê cà phê.", // Description
                        imageUrl: logo, // Logo (Ảnh đại diện trang web)
                        keywords: "công thức pha chế cà phê, pha chế cà phê ngon, bí quyết pha cà phê", // Keywords,
                        updateTime: null, // UpdateTime (Có thể dùng thời gian cập nhật nếu có)
                        tag: "công thức, pha chế, cà phê ngon" // Tag (Các thẻ liên quan)
                    );

            ViewBag.Header = SetMetaTags(metaTag);
            #endregion

            return await HandleBlogRequest(model, type: BlogTypeEnum.FORMULA, nameof(Formula));
        }


        [Route("/tin-tuc/{category}/{code}", Name = "blog-detail")]
        [Route("/kien-thuc/{category}/{code}", Name = "know-detail")]
        [Route("/cong-thuc/{category}/{code}", Name = "formula-detail")]
        public async Task<IActionResult> Detail(string code)
        {
            var blog = await _blogService.FindBlogByCodeAsync(code);
            if (blog == null)
            {
                return RedirectToRoute("error", new { code = StatusCodes.Status404NotFound });
            }
            var logo = await _aboutService.GetLogoTopCacheAsync() ?? Commons.LOGO_TOP;
            // Từ khóa SEO mặc định
            var defaultKeywords = "cà phê rang xay, cà phê nguyên chất, Cao Gia Construction, cà phê chất lượng cao, cà phê specialty";
            #region Seo Meta Tag
            var metaTag = BuildMetaTag(
                          title: !string.IsNullOrEmpty(blog.Title) ? blog.Title : "Thưởng thức cà phê chất lượng tại Cao Gia Construction", // Title (Thêm tiêu đề trang chứa từ khóa chính)
                          siteName: "Cao Gia Construction", // SiteName (Tên trang web hoặc công ty)
                          pageType: "article", // PageType (Loại trang: product, article)
                          description: !string.IsNullOrEmpty(blog.Description)
                            ? blog.Description
                            : "Cao Gia Construction mang đến cà phê rang xay nguyên chất, chất lượng cao, kết hợp cùng các thức uống sáng tạo.", // Description
                         imageUrl: logo, // Logo (Ảnh đại diện trang web)
                         keywords: !string.IsNullOrEmpty(blog.SeoKeywords) ? blog.SeoKeywords : defaultKeywords, // Keywords,
                         updateTime: blog.ModifiedDate?.ToString("yyyy-MM-ddTHH:mm:ssZ"), // UpdateTime
                         tag: !string.IsNullOrEmpty(blog.SeoKeywords) ? blog.SeoKeywords : defaultKeywords // Tag (Các thẻ liên quan)
                        );

            ViewBag.Header = SetMetaTags(metaTag);
            #endregion

            // Lấy danh sách bài viết liên quan
            ViewBag.BlogRelateds = await _blogService.GetBlogRelatedsync(blog.BlogCategoryId.ToGuid(), blog.Id.ToGuid());

            return View(blog);
        }

        private async Task<IActionResult> HandleBlogRequest(SearchBlogClientDto model, BlogTypeEnum type, string viewName)
        {
            model.PageSize = 6;
            ViewBag.Param = model;
            model.Type = type;

            var data = await _blogService.GetPaginationBlogClientAsync(model);

            return View(viewName, data);
        }
    }
}
