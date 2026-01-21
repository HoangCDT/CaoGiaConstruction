using Microsoft.AspNetCore.Mvc;
using CaoGiaConstruction.Utilities.Constants;
using CaoGiaConstruction.WebClient.Extensions;
using CaoGiaConstruction.WebClient.Services;
using static CaoGiaConstruction.Utilities.SetMetaTagUtility;

namespace CaoGiaConstruction.WebClient.Controllers
{
    public class AboutController : BaseClientController
    {
        private readonly IAboutService _aboutService;

        public AboutController(IAboutService aboutService)
        {
            _aboutService = aboutService;
        }

        [Route("/ve-chung-toi", Name = "about")]
        public async Task<IActionResult> About()
        {
            var about = await _aboutService.GetAboutCacheAsync();

            var metaTag = BuildMetaTag(
                          title: !string.IsNullOrEmpty(about.AboutUs) ? about.AboutUs : "Tìm hiểu về Cao Gia Construction - công ty xây dựng uy tín, chuyên nghiệp và chất lượng cao.",
                          siteName: "Cao Gia Construction", // Site name
                          pageType: "about",
                          description: !string.IsNullOrEmpty(about.Description) ? about.Description : "Tìm hiểu về Cao Gia Construction - công ty xây dựng uy tín, chuyên nghiệp và chất lượng cao.",
                          imageUrl: about.LogoTop,
                          keywords: !string.IsNullOrEmpty(about.SeoKeywords) ? about.SeoKeywords : "Cao Gia Construction, xây dựng, thi công công trình, xây dựng chất lượng cao, công ty xây dựng uy tín",
                          updateTime: null,
                          tag: !string.IsNullOrEmpty(about.SeoKeywords) ? about.SeoKeywords : "Cao Gia Construction, xây dựng, thi công công trình, xây dựng chất lượng cao, công ty xây dựng uy tín"
                      );

            ViewBag.Header = SetMetaTags(metaTag);

            return View(about);
        }
    }
}