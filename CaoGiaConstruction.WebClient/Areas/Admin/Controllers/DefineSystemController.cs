using Microsoft.AspNetCore.Mvc;
using CaoGiaConstruction.WebClient.Areas.Admin.Controllers;
using CaoGiaConstruction.WebClient.Areas.Admin.Dtos;
using CaoGiaConstruction.WebClient.Context.Entities;
using CaoGiaConstruction.WebClient.Services;

public class DefineSystemController : BaseController
{
    private readonly IDefineSystemService _service;

    public DefineSystemController(IDefineSystemService service)
    {
        _service = service;
    }

    [Route("/{area}/define-system", Name = "admin-define-system")]
    public async Task<IActionResult> Index(SearchKeywordPagination model)
    {
        model.PageSize = 5;
        ViewBag.Keyword = model.Keyword;
        var blogCategories = await _service.GetPaginationAsync(model);

        return View(blogCategories);
    }

    [Route("/{area}/define-system/action", Name = "admin-define-system-action-add")]
    [Route("/{area}/define-system/{id}/action", Name = "admin-define-system-action")]
    public IActionResult Action()
    {
        return View();
    }

    [HttpPut]
    [Route("/{area}/define-system/{id}/status")]
    public async Task<JsonResult> UpdateStatus(Guid id)
    {
        var result = await _service.ChangeStatus(id);
        return Json(result);
    }

    [HttpDelete]
    [Route("/{area}/define-system/{id}/delete")]
    public async Task<JsonResult> Delete(Guid id)
    {
        var result = await _service.RemoveAsync(id);
        return Json(result);
    }

    [HttpPost]
    [Route("/{area}/define-system/addorupdate")]
    public async Task<JsonResult> AddOrUpdate([FromBody] DefineSystem model)
    {
        var result = await _service.AddOrUpdateAsync(model);
        return Json(result);
    }

    [HttpGet]
    [Route("/{area}/define-system/{id}")]
    public async Task<JsonResult> FindById(Guid id)
    {
        var result = await _service.FindByIdAsync(id);
        return Json(result);
    }
}