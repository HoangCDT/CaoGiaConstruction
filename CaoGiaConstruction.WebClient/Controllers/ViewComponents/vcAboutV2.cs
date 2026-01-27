using Microsoft.AspNetCore.Mvc;
using CaoGiaConstruction.WebClient.AutoMapper.ViewModels;
using CaoGiaConstruction.WebClient.Const;
using CaoGiaConstruction.WebClient.Services;

namespace CaoGiaConstruction.WebClient.Controllers.ViewComponents
{
    public class vcAboutV2 : ViewComponent
    {
        private readonly IAboutService _aboutService;
        private readonly ISlideService _slideService;
        private readonly ISettingService _settingService;

        public vcAboutV2(IAboutService aboutService, ISlideService slideService, ISettingService settingService)
        {
            _aboutService = aboutService;
            _slideService = slideService;
            _settingService = settingService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = new AboutV2VM
            {
                About = await _aboutService.GetAboutCacheAsync(),
                Banner = await _slideService.GetActiveSlideByCategoryCodeAsync(SlideCategoryCodeDefine.HOME_BANNER_ABOUT),
                Setting = await _settingService.GetSettingCacheAsync()
            };

            return View(model);
        }
    }
}
