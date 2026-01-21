using Microsoft.AspNetCore.Mvc;
using CaoGiaConstruction.Utilities.Constants;
using CaoGiaConstruction.WebClient.Context.Enums;
using CaoGiaConstruction.WebClient.Services;
using static CaoGiaConstruction.Utilities.SetMetaTagUtility;

namespace CaoGiaConstruction.WebClient.Controllers
{
    public class BranchController : BaseClientController
    {
        private readonly IBranchesService _branchesService;
        private readonly IAboutService _aboutService;

        public BranchController(IBranchesService branchesService, IAboutService aboutService)
        {
            _branchesService = branchesService;
            _aboutService = aboutService;
        }

        [Route("/chi-nhanh", Name = "branch")]
        public async Task<IActionResult> Index()
        {
            var branch = await _branchesService.GetAllAsync(x => x.Status == StatusEnum.Active);
            var logo = await _aboutService.GetLogoTopCacheAsync() ?? Commons.LOGO_TOP;

            #region Seo Meta Tag
            var metaTag = BuildMetaTag(
             title:  "Danh sách chi nhánh Cao Gia Construction trên toàn quốc", // Title (Thêm tiêu đề trang chứa từ khóa chính)
             siteName: "Cao Gia Construction", // SiteName (Tên trang web hoặc công ty)
             pageType: "branch", // PageType (Loại trang: product, article)
             description: "Khám phá danh sách chi nhánh Cao Gia Construction trên toàn quốc. Tìm hiểu dịch vụ xây dựng chuyên nghiệp và chất lượng cao tại các địa điểm gần bạn.", // Description
             imageUrl: logo, // Logo (Ảnh đại diện trang web)
             keywords: "Cao Gia Construction chi nhánh, địa điểm xây dựng, dịch vụ xây dựng, chi nhánh Cao Gia Construction toàn quốc", // Keywords,
             updateTime: null, // UpdateTime
             tag: "Cao Gia Construction chi nhánh, địa điểm xây dựng, dịch vụ xây dựng, chi nhánh Cao Gia Construction toàn quốc" // Tag (Các thẻ liên quan)
            );

            ViewBag.Header = SetMetaTags(metaTag);
            #endregion

            return View(branch);
        }

    }
}