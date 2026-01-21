using Microsoft.AspNetCore.Mvc;
using static CaoGiaConstruction.Utilities.SetMetaTagUtility;
using CaoGiaConstruction.WebClient.Extensions;
using CaoGiaConstruction.WebClient.Services;
using CaoGiaConstruction.Utilities.Constants;
using CaoGiaConstruction.WebClient.Context.Entities;

namespace CaoGiaConstruction.WebClient.Controllers
{
    public class HomeController : BaseClientController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IAboutService _aboutService;

        public HomeController(ILogger<HomeController> logger, IAboutService aboutService)
        {
            _logger = logger;
            _aboutService = aboutService;
        }

        [Route("", Name = "default")]
        [Route("trang-chu", Name = "home")]
        public async Task<IActionResult> IndexAsync()
        {
            var logo = await _aboutService.GetLogoTopCacheAsync() ?? Commons.LOGO_TOP;

            #region Seo Meta Tag
            var metaTag = BuildMetaTag(
               title: "Cao Gia Construction", // Title (Thêm tiêu đề trang chứa từ khóa chính)
               siteName: "Cao Gia Construction", // SiteName (Tên trang web hoặc công ty)
               pageType: "home", // PageType (Loại trang: product, article)
               description: "Khám phá dịch vụ xây dựng chuyên nghiệp tại Cao Gia Construction, kết hợp với các giải pháp thi công hiện đại và dịch vụ uy tín.", // Description
               imageUrl: logo, // Logo (Ảnh đại diện trang web)
               keywords: "xây dựng, Cao Gia Construction, thi công công trình, xây dựng chất lượng cao, công ty xây dựng", // Keywords,
               updateTime: null, // UpdateTime
               tag: "xây dựng, Cao Gia Construction, thi công công trình, xây dựng chất lượng cao" // Tag (Các thẻ liên quan)
            );

            ViewBag.Header = SetMetaTags(metaTag);
            #endregion

            return View();
        }

        [HttpGet]
        [Route("api/tiktok-embed")]
        public async Task<IActionResult> GetTiktokEmbedAsync()
        {
            try
            {
                // Render partial view _TiktokEmbed from Shared folder
                var partialView = await this.RenderViewAsync("_TiktokEmbed", null, true);
                if (string.IsNullOrEmpty(partialView))
                {
                    return Json(new { success = false, html = "" });
                }
                return Json(new { success = true, html = partialView });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading TikTok embed");
                return Json(new { success = false, html = "" });
            }
        }
        
    }
}