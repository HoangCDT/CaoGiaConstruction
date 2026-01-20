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

        public vcAboutHome(ISlideService slideService, IAboutService aboutService)
        {
            _slideService = slideService;
            _aboutService = aboutService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = new AboutHomeVM
            {
                Banner = await _slideService.GetActiveSlideByCategoryCodeAsync(SlideCategoryCodeDefine.HOME_BANNER_ABOUT),
                About = await _aboutService.GetAboutCacheAsync()
            };

            return View(model);
        }
    }
}