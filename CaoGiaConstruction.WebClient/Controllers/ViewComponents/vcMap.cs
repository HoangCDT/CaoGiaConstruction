using Microsoft.AspNetCore.Mvc;
using CaoGiaConstruction.WebClient.Services;
using CaoGiaConstruction.WebClient.AutoMapper.ViewModels;

namespace CaoGiaConstruction.WebClient.Controllers.ViewComponents
{
    public class vcMap : ViewComponent
    {
        private readonly IAboutService _aboutService;

        public vcMap(IAboutService aboutService)
        {
            _aboutService = aboutService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string mapIFrame = null, string cssClass = "about-v2-map")
        {
            // Nếu không truyền mapIFrame, lấy từ About service
            if (string.IsNullOrEmpty(mapIFrame))
            {
                var about = await _aboutService.GetAboutCacheAsync();
                mapIFrame = about?.MapIFrame;
            }

            // Xử lý extract URL từ iframe tag nếu cần
            string mapUrl = mapIFrame ?? string.Empty;
            if (!string.IsNullOrEmpty(mapUrl))
            {
                // Nếu MapIFrame chứa thẻ iframe, extract URL từ src
                if (mapUrl.Contains("<iframe"))
                {
                    var srcMatch = System.Text.RegularExpressions.Regex.Match(mapUrl, @"src=[""']([^""']+)[""']");
                    if (srcMatch.Success)
                    {
                        mapUrl = srcMatch.Groups[1].Value;
                    }
                }
                // Nếu vẫn chứa thẻ iframe, thử extract từ src= trực tiếp
                if (mapUrl.Contains("src="))
                {
                    var srcMatch = System.Text.RegularExpressions.Regex.Match(mapUrl, @"src=[""']([^""']+)[""']");
                    if (srcMatch.Success)
                    {
                        mapUrl = srcMatch.Groups[1].Value;
                    }
                }
            }

            var model = new MapVM
            {
                MapUrl = mapUrl,
                CssClass = cssClass
            };

            return View(model);
        }
    }

    public class MapVM
    {
        public string MapUrl { get; set; }
        public string CssClass { get; set; }
    }
}
