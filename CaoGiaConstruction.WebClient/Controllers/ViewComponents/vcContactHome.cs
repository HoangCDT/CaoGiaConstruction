using Microsoft.AspNetCore.Mvc;
using CaoGiaConstruction.WebClient.Services;
using CaoGiaConstruction.WebClient.AutoMapper.ViewModels;

namespace CaoGiaConstruction.WebClient.Controllers.ViewComponents
{
    public class vcContactHome : ViewComponent
    {
        private readonly IAboutService _aboutService;

        public vcContactHome(IAboutService aboutService)
        {
            _aboutService = aboutService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var about = await _aboutService.GetAboutCacheAsync();

            var model = new ContactHomeVM
            {
                About = about,
                Setting = ViewBag.Setting as SettingVM
            };

            return View(model);
        }
    }

    public class ContactHomeVM
    {
        public AboutVM About { get; set; }
        public SettingVM Setting { get; set; }
    }
}
