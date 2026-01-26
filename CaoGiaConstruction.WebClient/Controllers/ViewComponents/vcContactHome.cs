using Microsoft.AspNetCore.Mvc;
using CaoGiaConstruction.WebClient.Services;
using CaoGiaConstruction.WebClient.AutoMapper.ViewModels;

namespace CaoGiaConstruction.WebClient.Controllers.ViewComponents
{
    public class vcContactHome : ViewComponent
    {
        private readonly IAboutService _aboutService;
        private readonly ISettingService _settingService;

        public vcContactHome(IAboutService aboutService, ISettingService settingService)
        {
            _aboutService = aboutService;
            _settingService = settingService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var about = await _aboutService.GetAboutCacheAsync();
            var setting = await _settingService.GetSettingCacheAsync();

            var model = new ContactHomeVM
            {
                About = about,
                Setting = setting
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
