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
                        title: "Tin tức mới nhất về xây dựng - Cao Gia Construction", // Title (Thêm tiêu đề trang chứa từ khóa chính)
                        siteName: "Cao Gia Construction - Chuyên xây dựng chất lượng cao", // SiteName (Tên trang web hoặc công ty)
                        pageType: "article", // PageType (Loại trang: product, article)
                        description: "Cập nhật các tin tức và sự kiện mới nhất về xây dựng, công trình, và những dự án đặc biệt tại Cao Gia Construction.", // Description
                        imageUrl: logo, // Logo (Ảnh đại diện trang web)
                        keywords: "xây dựng, tin tức xây dựng, Cao Gia Construction, công trình xây dựng, dự án xây dựng", // Keywords,
                        updateTime: null, // UpdateTime
                        tag: "tin tức xây dựng, sự kiện xây dựng, công trình xây dựng" // Tag (Các thẻ liên quan)
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
                        title: "Kiến thức xây dựng - Tìm hiểu mọi điều về xây dựng | Cao Gia Construction", // Title (Thêm tiêu đề trang chứa từ khóa chính)
                        siteName: "Cao Gia Construction", // SiteName (Tên trang web hoặc công ty)
                        pageType: "knowledge", // PageType (Loại trang: product, article)
                        description: "Khám phá mọi kiến thức về xây dựng tại Cao Gia Construction. Chúng tôi chia sẻ những kinh nghiệm thi công, kỹ thuật xây dựng, và nhiều điều bổ ích.", // Description
                        imageUrl: logo, // Logo (Ảnh đại diện trang web)
                        keywords: "kiến thức xây dựng, kỹ thuật xây dựng, kinh nghiệm thi công", // Keywords,
                        updateTime: null, // UpdateTime
                        tag: "kiến thức, xây dựng, kỹ thuật thi công" // Tag (Các thẻ liên quan)
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
                        title: "Công Thức Xây Dựng - Bí Quyết Tính Toán | Cao Gia Construction", // Title (Thêm tiêu đề trang chứa từ khóa chính)
                        siteName: "Cao Gia Construction", // SiteName (Tên trang web hoặc công ty)
                        pageType: "article", // PageType (Loại trang: product, article)
                        description: "Khám phá công thức tính toán trong xây dựng và thi công tại Cao Gia Construction. Cùng chúng tôi tìm hiểu những phương pháp tính toán chính xác để đảm bảo chất lượng công trình.", // Description
                        imageUrl: logo, // Logo (Ảnh đại diện trang web)
                        keywords: "công thức xây dựng, tính toán xây dựng, bí quyết thi công", // Keywords,
                        updateTime: null, // UpdateTime (Có thể dùng thời gian cập nhật nếu có)
                        tag: "công thức, tính toán, xây dựng" // Tag (Các thẻ liên quan)
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
            var defaultKeywords = "xây dựng, thi công công trình, Cao Gia Construction, xây dựng chất lượng cao, công ty xây dựng uy tín";
            #region Seo Meta Tag
            var metaTag = BuildMetaTag(
                          title: !string.IsNullOrEmpty(blog.Title) ? blog.Title : "Dịch vụ xây dựng chất lượng tại Cao Gia Construction", // Title (Thêm tiêu đề trang chứa từ khóa chính)
                          siteName: "Cao Gia Construction", // SiteName (Tên trang web hoặc công ty)
                          pageType: "article", // PageType (Loại trang: product, article)
                          description: !string.IsNullOrEmpty(blog.Description)
                            ? blog.Description
                            : "Cao Gia Construction mang đến dịch vụ xây dựng chuyên nghiệp, chất lượng cao, kết hợp cùng các giải pháp thi công hiện đại.", // Description
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
