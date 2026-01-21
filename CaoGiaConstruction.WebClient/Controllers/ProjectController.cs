using Microsoft.AspNetCore.Mvc;
using CaoGiaConstruction.Utilities;
using CaoGiaConstruction.Utilities.Constants;
using CaoGiaConstruction.WebClient.Dtos;
using CaoGiaConstruction.WebClient.Extensions;
using CaoGiaConstruction.WebClient.Services;
using static CaoGiaConstruction.Utilities.SetMetaTagUtility;

namespace CaoGiaConstruction.WebClient.Controllers
{
    public class ProjectController : BaseClientController
    {
        private readonly ILogger<ProjectController> _logger;
        private readonly IAboutService _aboutService;
        private readonly IProjectService _project;

        public ProjectController(ILogger<ProjectController> logger, IProjectService project, IAboutService aboutService)
        {
            _logger = logger;
            _project = project;
            _aboutService = aboutService;
        }

        [Route("/du-an", Name = "project")]
        [Route("/du-an/{code}", Name = "project-cate")]
        public async Task<IActionResult> Project(SearchPaginationDto model)
        {
            model.PageSize = 12;
            ViewBag.Param = model;

            var data = await _project.GetPaginationProjectClientAsync(model);
            var logo = await _aboutService.GetLogoTopCacheAsync();

            #region Seo Meta Tag
            var metaTag = BuildMetaTag(
                title: "Dự án của Cao Gia Construction", // Dynamic title based on the product title
                siteName: "Cao Gia Construction", // Site name
                pageType: "project", // Page type: product-detail (for detailed product page)
                description: "Danh sách các dự án của Cao Gia Construction, bao gồm các công trình xây dựng, dự án thi công và giải pháp xây dựng chuyên nghiệp.", // Dynamic description
                imageUrl: logo, // Image URL (avatar or first image in the list)
                keywords: "Dự án Cao Gia Construction, công trình xây dựng, dự án thi công, giải pháp xây dựng", // Dynamic keywords
                updateTime: null, // Current update time in ISO format
                tag: "Dự án, Cao Gia Construction, công trình xây dựng, dự án thi công, giải pháp xây dựng" // Tags related to the product
            );

            // Set meta tags for the product detail page
            ViewBag.Header = SetMetaTags(metaTag);

            #endregion

            return View(data);
        }

        [Route("/du-an/{category}/{code}", Name = "project-detail")]
        public async Task<IActionResult> Detail(string code)
        {

            var project = await _project.FindProjectByCodeAsync(code);

            if (project == null)
            {
                return RedirectToRoute("error", new { code = StatusCodes.Status404NotFound });
            }

            ViewBag.ProjectRelateds = await _project.GetProjectRelatedsync(project.ServiceId.ToGuid(), project.Id.ToGuid());

            #region Seo Meta Tag
            var metaTag = BuildMetaTag(
                title: !string.IsNullOrEmpty(project.Title) ? project.Title : $"{project.Title} - Dự án tại Cao Gia Construction", // Dynamic title based on the product title
                siteName: "Cao Gia Construction", // Site name
                pageType: "project-detail", // Page type: product-detail (for detailed product page)
                description: !string.IsNullOrEmpty(project.Description) ? project.Description :
                "Dự án của Cao Gia Construction, nơi bạn sẽ khám phá những công trình xây dựng chất lượng và giải pháp thi công hiện đại.", // Dynamic description
                imageUrl: project.Avatar, // Image URL (avatar or first image in the list)
                keywords: !string.IsNullOrEmpty(project.SeoKeywords) ? project.SeoKeywords : "Dự án Cao Gia Construction, công trình xây dựng, dự án thi công, giải pháp xây dựng", // Dynamic keywords
                updateTime: project.ModifiedDate?.ToString("yyyy-MM-ddTHH:mm:ssZ"), // Current update time in ISO format
                tag: !string.IsNullOrEmpty(project.SeoKeywords) ? project.SeoKeywords : "Dự án Cao Gia Construction, công trình xây dựng, dự án thi công, giải pháp xây dựng" // Tags related to the product
            );
            // Set meta tags for the product detail page
            ViewBag.Header = SetMetaTags(metaTag);
            #endregion

            return View(project);
        }
    }
}