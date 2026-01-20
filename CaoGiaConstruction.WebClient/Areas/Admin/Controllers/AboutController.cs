using Microsoft.AspNetCore.Mvc;
using CaoGiaConstruction.WebClient.AutoMapper.ViewModels;
using CaoGiaConstruction.WebClient.Services;

namespace CaoGiaConstruction.WebClient.Areas.Admin.Controllers
{
    public class AboutController : BaseController
    {
        private readonly IAboutService _aboutService;

        public AboutController(IAboutService aboutService)
        {
            _aboutService = aboutService;
        }

        [Route("/{area}/about", Name = "admin-about")]
        public async Task<IActionResult> Index()
        {
            var about = await _aboutService.GetLatestAboutAsync();
            return View(about);
        }

        [Route("/{area}/about/update", Name = "admin-blog-action-update")]
        public async Task<JsonResult> UpdateAbout([FromForm] AboutActionVM model)
        {
            var result = await _aboutService.UpdateAboutAsync(model);
            return Json(result);
        }
    }
}