using Microsoft.AspNetCore.Mvc;
using CaoGiaConstruction.WebClient.AutoMapper.ViewModels;
using CaoGiaConstruction.WebClient.Const;
using CaoGiaConstruction.WebClient.Services;

namespace CaoGiaConstruction.WebClient.Controllers.ViewComponents
{
    public class vcAboutHome : ViewComponent
    {
        private readonly IAboutService _aboutService;
        private readonly ISlideService _slideService;
        private readonly ISettingService _settingService;

        public vcAboutHome(ISlideService slideService, IAboutService aboutService, ISettingService settingService)
        {
            _slideService = slideService;
            _aboutService = aboutService;
            _settingService = settingService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = new AboutHomeVM
            {
                Banner = await _slideService.GetActiveSlideByCategoryCodeAsync(SlideCategoryCodeDefine.HOME_BANNER_ABOUT),
                About = await _aboutService.GetAboutCacheAsync(),
                Setting = await _settingService.GetSettingCacheAsync()
            };

            return View(model);
        }
    }
}