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

        public vcAboutV2(IAboutService aboutService, ISlideService slideService)
        {
            _aboutService = aboutService;
            _slideService = slideService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = new AboutV2VM
            {
                About = await _aboutService.GetAboutCacheAsync(),
                Banner = await _slideService.GetActiveSlideByCategoryCodeAsync(SlideCategoryCodeDefine.HOME_BANNER_ABOUT),
                Setting = ViewBag.Setting as SettingVM
            };

            return View(model);
        }
    }
}
