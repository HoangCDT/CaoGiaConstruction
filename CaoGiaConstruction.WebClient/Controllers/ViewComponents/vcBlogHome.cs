using Microsoft.AspNetCore.Mvc;
using CaoGiaConstruction.WebClient.AutoMapper.ViewModels;
using CaoGiaConstruction.WebClient.Const;
using CaoGiaConstruction.WebClient.Context.Enums;
using CaoGiaConstruction.WebClient.Services;

namespace CaoGiaConstruction.WebClient.Controllers.ViewComponents
{
    public class vcBlogHome : ViewComponent
    {
        private readonly IBlogService _serviceBlog;
        private readonly ISlideService _serviceSlide;

        public vcBlogHome(IBlogService serviceBlog, ISlideService serviceSlide)
        {
            _serviceBlog = serviceBlog;
            _serviceSlide = serviceSlide;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = new HomeVM
            {
                HotBlogs = await _serviceBlog.GetHotBlogHomeAsync(BlogTypeEnum.NEWSEVENT),
                HomeBlogs = await _serviceBlog.GetTopHomeBlogsAsync(1, BlogTypeEnum.KNOWLEDGE),
                BannerBlog = await _serviceSlide.GetActiveSlideByCategoryCodeAsync(SlideCategoryCodeDefine.HOME_BANNER_BLOG)
            };

            return View(model);
        }
    }
}