using Microsoft.AspNetCore.Mvc;
using CaoGiaConstruction.Utilities.Constants;
using CaoGiaConstruction.WebClient.Const;
using CaoGiaConstruction.WebClient.Extensions;
using CaoGiaConstruction.WebClient.Services;
using CaoGiaConstruction.WebClient.ViewModels;
using CaoGiaConstruction.WebClient.Context.Entities;
using Microsoft.EntityFrameworkCore;
using static CaoGiaConstruction.Utilities.SetMetaTagUtility;

namespace CaoGiaConstruction.WebClient.Controllers
{
    public class AboutController : BaseClientController
    {
        private readonly IAboutService _aboutService;
        private readonly ITeamMemberService _teamMemberService;
        private readonly ISlideService _slideService;
        private readonly ITimeLineService _timeLineService;
        private readonly ICoreValueService _coreValueService;

        public AboutController(
            IAboutService aboutService,
            ITeamMemberService teamMemberService,
            ISlideService slideService,
            ITimeLineService timeLineService,
            ICoreValueService coreValueService)
        {
            _aboutService = aboutService;
            _teamMemberService = teamMemberService;
            _slideService = slideService;
            _timeLineService = timeLineService;
            _coreValueService = coreValueService;
        }

        [Route("/ve-chung-toi", Name = "about")]
        public async Task<IActionResult> AboutPageV2()
        {
            var aboutSettings = await _aboutService.GetAboutCacheAsync();
            var founder = await _teamMemberService.GetFounderAsync();
            var teamMembers = await _teamMemberService.GetAllMembersAsync();
            
            // Getting Milestones - assuming similar GetAll approach or predicate
            var milestones = await _timeLineService.GetAllAsync(); // Or sorted by date
            milestones = milestones.OrderBy(x => x.EventDate).ToList(); // Simple sort, refine if needed based on date format

            // Getting Core Values and Partners using SlideService
            var slidesWithCategory = _slideService.AsQueryable()
                .Include(x => x.SlideCategory)
                .Where(x => x.Status == Context.Enums.StatusEnum.Active)
                .ToList();

            var coreValues = await _coreValueService.GetActiveCoreValuesAsync();
            var partners = slidesWithCategory.Where(x => x.SlideCategory?.Code == "HOME_SLIDE_PARTNER" || x.SlideCategory?.Title?.Contains("Đối tác") == true).ToList();
            
            // Get About slide banner
            var aboutSlide = await _slideService.GetActiveSlideByCategoryCodeAsync(SlideCategoryCodeDefine.HOME_BANNER_ABOUT);

            // Set meta tags
            var metaTag = BuildMetaTag(
                title: !string.IsNullOrEmpty(aboutSettings.AboutUs) ? aboutSettings.AboutUs : "Về Chúng Tôi | Xây Dựng Cao Gia",
                siteName: "Cao Gia Construction",
                pageType: "about",
                description: !string.IsNullOrEmpty(aboutSettings.Description) ? aboutSettings.Description : "Tìm hiểu về Cao Gia Construction - công ty xây dựng uy tín, chuyên nghiệp và chất lượng cao.",
                imageUrl: aboutSettings.LogoTop,
                keywords: !string.IsNullOrEmpty(aboutSettings.SeoKeywords) ? aboutSettings.SeoKeywords : "Cao Gia Construction, xây dựng, thi công công trình, xây dựng chất lượng cao, công ty xây dựng uy tín",
                updateTime: null,
                tag: !string.IsNullOrEmpty(aboutSettings.SeoKeywords) ? aboutSettings.SeoKeywords : "Cao Gia Construction, xây dựng, thi công công trình, xây dựng chất lượng cao, công ty xây dựng uy tín"
            );

            ViewBag.Header = SetMetaTags(metaTag);

            return View("AboutV2", new AboutViewModel 
            {
                AboutSettings = await _aboutService.FindByIdAsync(aboutSettings.Id ?? Guid.Empty), 
                Founder = founder,
                TeamMembers = teamMembers,
                Milestones = milestones,
                CoreValues = coreValues,
                Partners = partners,
                AboutSlide = aboutSlide
            });
        }
    }
}