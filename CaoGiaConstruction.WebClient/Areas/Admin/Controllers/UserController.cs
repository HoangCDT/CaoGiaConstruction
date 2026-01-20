using Microsoft.AspNetCore.Mvc;
using CaoGiaConstruction.WebClient.Areas.Admin.Dtos;
using CaoGiaConstruction.WebClient.AutoMapper.ViewModels;
using CaoGiaConstruction.WebClient.Services;

namespace CaoGiaConstruction.WebClient.Areas.Admin.Controllers
{
    public class UserController : BaseController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [Route("/{area}/user", Name = "admin-user")]
        public async Task<IActionResult> Index(SearchKeywordPagination model)
        {
            model.PageSize = 5;
            ViewBag.Keyword = model.Keyword;
            var blogCategories = await _userService.GetPaginationAsync(model);
            return View(blogCategories);
        }

        [Route("/{area}/user/action", Name = "admin-user-action-add")]
        [Route("/{area}/user/{id}/action", Name = "admin-user-action")]
        public IActionResult Action()
        {
            return View();
        }

        [HttpPut]
        [Route("/{area}/user/{id}/status")]
        public async Task<JsonResult> UpdateStatus(Guid id)
        {
            var result = await _userService.ChangeStatus(id);
            return Json(result);
        }

        [HttpDelete]
        [Route("/{area}/user/{id}/delete")]
        public async Task<JsonResult> Delete(Guid id)
        {
            var result = await _userService.RemoveAsync(id);
            return Json(result);
        }

        [HttpPost]
        [Route("/{area}/user/addorupdate")]
        public async Task<JsonResult> AddOrUpdate([FromForm] UserActionVM model)
        {
            var result = await _userService.AddOrUpdateActionAsync(model);
            return Json(result);
        }

        [HttpGet]
        [Route("/{area}/user/{id}")]
        public async Task<JsonResult> FindById(Guid id)
        {
            var result = await _userService.FindByIdAsync(id);
            return Json(result);
        }
    }
}