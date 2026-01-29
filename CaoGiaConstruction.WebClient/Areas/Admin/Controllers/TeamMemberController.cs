using Microsoft.AspNetCore.Mvc;
using CaoGiaConstruction.WebClient.Areas.Admin.Controllers;
using CaoGiaConstruction.WebClient.Areas.Admin.Dtos;
using CaoGiaConstruction.WebClient.AutoMapper.ViewModels;
using CaoGiaConstruction.WebClient.Context.Entities;
using CaoGiaConstruction.WebClient.Services;

namespace CaoGiaConstruction.WebClient.Areas.Admin.Controllers
{
    public class TeamMemberController : BaseController
    {
        private readonly ITeamMemberService _service;

        public TeamMemberController(ITeamMemberService service)
        {
            _service = service;
        }

        [Route("/{area}/team-member", Name = "admin-team-member")]
        public async Task<IActionResult> Index(SearchKeywordPagination model)
        {
            model.PageSize = 5;
            ViewBag.Keyword = model.Keyword;
            var teamMembers = await _service.GetPaginationAsync(model);
            return View(teamMembers);
        }


        [HttpPut]
        [Route("/{area}/team-member/{id}/status")]
        public async Task<JsonResult> UpdateStatus(Guid id)
        {
            var result = await _service.ChangeStatus(id);
            return Json(result);
        }

        [HttpDelete]
        [Route("/{area}/team-member/{id}/delete")]
        public async Task<JsonResult> Delete(Guid id)
        {
            var result = await _service.RemoveAsync(id);
            return Json(result);
        }

        [HttpPost]
        [Route("/{area}/team-member/addorupdate")]
        public async Task<JsonResult> AddOrUpdate([FromForm] TeamMemberActionVM model)
        {
            var result = await _service.AddOrUpdateActionAsync(model);
            return Json(result);
        }

        [HttpGet]
        [Route("/{area}/team-member/{id}")]
        public async Task<JsonResult> FindById(Guid id)
        {
            var result = await _service.FindByIdAsync(id);
            return Json(result);
        }

        [HttpPost]
        [Route("/{area}/team-member/sort")]
        public async Task<JsonResult> UpdateSort([FromBody] List<TeamMemberSortDto> items)
        {
            var result = await _service.UpdateSortOrderAsync(items);
            return Json(result);
        }
    }
}
