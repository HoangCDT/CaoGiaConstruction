using Microsoft.AspNetCore.Mvc;
using CaoGiaConstruction.WebClient.Areas.Admin.Dtos;
using CaoGiaConstruction.WebClient.Services;

namespace CaoGiaConstruction.WebClient.Areas.Admin.Controllers
{
    public class ContactController : BaseController
    {
        private readonly IContactService _contactService;

        public ContactController(IContactService contactService)
        {
            _contactService = contactService;
        }

        [Route("/{area}/contact", Name = "admin-contact")]
        public async Task<IActionResult> Index(SearchKeywordPagination model)
        {
            model.PageSize = 5;
            ViewBag.Keyword = model.Keyword;
            var blogCategories = await _contactService.GetPaginationAsync(model);
            return View(blogCategories);
        }

        [Route("/{area}/contact/action", Name = "admin-contact-action-add")]
        [Route("/{area}/contact/{id}/action", Name = "admin-contact-action")]
        public IActionResult Action()
        {
            return View();
        }

        [HttpPut]
        [Route("/{area}/contact/{id}/status")]
        public async Task<JsonResult> UpdateStatus(Guid id)
        {
            var result = await _contactService.ChangeStatus(id);
            return Json(result);
        }

        [HttpDelete]
        [Route("/{area}/contact/{id}/delete")]
        public async Task<JsonResult> Delete(Guid id)
        {
            var result = await _contactService.RemoveAsync(id);
            return Json(result);
        }

        [HttpGet]
        [Route("/{area}/contact/{id}")]
        public async Task<JsonResult> FindById(Guid id)
        {
            var result = await _contactService.FindByIdAsync(id);
            return Json(result);
        }
    }
}