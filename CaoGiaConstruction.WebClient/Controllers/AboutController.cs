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
                          title: !string.IsNullOrEmpty(about.AboutUs) ? about.AboutUs : "Tìm hiểu về Cao Gia Construction - nơi mang đến cà phê rang xay nguyên chất, sáng tạo và chất lượng cao.",
                          siteName: "Cao Gia Construction", // Site name
                          pageType: "about",
                          description: !string.IsNullOrEmpty(about.Description) ? about.Description : "Tìm hiểu về Cao Gia Construction - nơi mang đến cà phê rang xay nguyên chất, sáng tạo và chất lượng cao.",
                          imageUrl: about.LogoTop,
                          keywords: !string.IsNullOrEmpty(about.SeoKeywords) ? about.SeoKeywords : "Cao Gia Construction, cà phê rang xay, cà phê nguyên chất, thưởng thức cà phê, cà phê chất lượng cao",
                          updateTime: null,
                          tag: !string.IsNullOrEmpty(about.SeoKeywords) ? about.SeoKeywords : "Cao Gia Construction, cà phê rang xay, cà phê nguyên chất, thưởng thức cà phê, cà phê chất lượng cao"
                      );

            ViewBag.Header = SetMetaTags(metaTag);

            return View(about);
        }
    }
}