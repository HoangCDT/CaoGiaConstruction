using Microsoft.AspNetCore.Mvc;
using CaoGiaConstruction.WebClient.Areas.Admin.Dtos;
using CaoGiaConstruction.WebClient.AutoMapper.ViewModels;
using CaoGiaConstruction.WebClient.Services;

namespace CaoGiaConstruction.WebClient.Areas.Admin.Controllers
{
    public class FeedbackController : BaseController
    {
        private readonly IFeedbackService _feedbackService;

        public FeedbackController(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }

        [Route("/{area}/feedback", Name = "admin-feedback")]
        public async Task<IActionResult> Index(SearchKeywordPagination model)
        {
            model.PageSize = 5;
            ViewBag.Keyword = model.Keyword;
            var blogCategories = await _feedbackService.GetPaginationAsync(model);
            return View(blogCategories);
        }

        [Route("/{area}/feedback/action", Name = "admin-feedback-action-add")]
        [Route("/{area}/feedback/{id}/action", Name = "admin-feedback-action")]
        public IActionResult Action()
        {
            return View();
        }

        [HttpPut]
        [Route("/{area}/feedback/{id}/status")]
        public async Task<JsonResult> UpdateStatus(Guid id)
        {
            var result = await _feedbackService.ChangeStatus(id);
            return Json(result);
        }

        [HttpDelete]
        [Route("/{area}/feedback/{id}/delete")]
        public async Task<JsonResult> Delete(Guid id)
        {
            var result = await _feedbackService.RemoveAsync(id);
            return Json(result);
        }

        [HttpPost]
        [Route("/{area}/feedback/addorupdate")]
        public async Task<JsonResult> AddOrUpdate([FromForm] FeedbackActionVM model)
        {
            var result = await _feedbackService.AddOrUpdateActionAsync(model);
            return Json(result);
        }

        [HttpGet]
        [Route("/{area}/feedback/{id}")]
        public async Task<JsonResult> FindById(Guid id)
        {
            var result = await _feedbackService.FindByIdAsync(id);
            return Json(result);
        }
    }
}