using Microsoft.AspNetCore.Mvc;
using CaoGiaConstruction.WebClient.Areas.Admin.Controllers;
using CaoGiaConstruction.WebClient.Areas.Admin.Dtos;
using CaoGiaConstruction.WebClient.Context.Entities;
using CaoGiaConstruction.WebClient.Services;

public class TimeLineController : BaseController
{
    private readonly ITimeLineService _service;

    public TimeLineController(ITimeLineService service)
    {
        _service = service;
    }

    [Route("/{area}/timeline", Name = "admin-timeline")]
    public async Task<IActionResult> Index(SearchKeywordPagination model)
    {
        model.PageSize = 5;
        ViewBag.Keyword = model.Keyword;
        var blogCategories = await _service.GetPaginationAsync(model);
        return View(blogCategories);
    }

    [Route("/{area}/timeline/action", Name = "admin-timeline-action-add")]
    [Route("/{area}/timeline/{id}/action", Name = "admin-timeline-action")]
    public IActionResult Action()
    {
        return View();
    }

    [HttpPut]
    [Route("/{area}/timeline/{id}/status")]
    public async Task<JsonResult> UpdateStatus(Guid id)
    {
        var result = await _service.ChangeStatus(id);
        return Json(result);
    }

    [HttpDelete]
    [Route("/{area}/timeline/{id}/delete")]
    public async Task<JsonResult> Delete(Guid id)
    {
        var result = await _service.RemoveAsync(id);
        return Json(result);
    }

    [HttpPost]
    [Route("/{area}/timeline/addorupdate")]
    public async Task<JsonResult> AddOrUpdate([FromBody] TimeLine model)
    {
        var result = await _service.AddOrUpdateAsync(model);
        return Json(result);
    }

    [HttpGet]
    [Route("/{area}/timeline/{id}")]
    public async Task<JsonResult> FindById(Guid id)
    {
        var result = await _service.FindByIdAsync(id);
        return Json(result);
    }
}