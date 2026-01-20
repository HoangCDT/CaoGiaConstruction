using Microsoft.AspNetCore.Mvc;
using CaoGiaConstruction.WebClient.Areas.Admin.Dtos;
using CaoGiaConstruction.WebClient.AutoMapper.ViewModels;
using CaoGiaConstruction.WebClient.Services;

namespace CaoGiaConstruction.WebClient.Areas.Admin.Controllers
{
    public class VideoController : BaseController
    {
        private readonly IVideoService _videoService;

        public VideoController(
            IVideoService productCategoryService
        )
        {
            _videoService = productCategoryService;
        }

        [Route("/{area}/video", Name = "admin-video")]
        public async Task<IActionResult> Index(SearchProductCatKeywordPagination model)
        {
            model.PageSize = 5;
            ViewBag.ParamSearch = model;

            var productCategories = await _videoService.GetPaginationAsync(model);
            return View(productCategories);
        }

        [Route("/{area}/video/action", Name = "admin-video-action-add")]
        [Route("/{area}/video/{id}/action", Name = "admin-video-action")]
        public IActionResult Action()
        {
            return View();
        }

        [HttpPut]
        [Route("/{area}/video/{id}/status")]
        public async Task<JsonResult> UpdateStatus(Guid id)
        {
            var result = await _videoService.ChangeStatus(id);
            return Json(result);
        }

        [HttpDelete]
        [Route("/{area}/video/{id}/delete")]
        public async Task<JsonResult> Delete(Guid id)
        {
            var result = await _videoService.RemoveAsync(id);
            return Json(result);
        }

        [HttpPost]
        [Route("/{area}/video/addorupdate")]
        public async Task<JsonResult> AddOrUpdate([FromForm] VideoActionVM model)
        {
            if (!string.IsNullOrEmpty(model.YouTubeURL))
            {
                var uri = new Uri(model.YouTubeURL);
                var idVideo = System.Web.HttpUtility.ParseQueryString(uri.Query).Get("v");
                model.YouTubeId = idVideo;
            }

            var result = await _videoService.AddOrUpdateActionAsync(model);
            return Json(result);
        }

        [HttpGet]
        [Route("/{area}/video/{id}")]
        public async Task<JsonResult> FindById(Guid id)
        {
            var result = await _videoService.FindByIdAsync(id);
            return Json(result);
        }
    }
}