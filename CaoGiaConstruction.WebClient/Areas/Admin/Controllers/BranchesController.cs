using Microsoft.AspNetCore.Mvc;
using CaoGiaConstruction.WebClient.Areas.Admin.Controllers;
using CaoGiaConstruction.WebClient.Areas.Admin.Dtos;
using CaoGiaConstruction.WebClient.AutoMapper.ViewModels;
using CaoGiaConstruction.WebClient.Context.Entities;
using CaoGiaConstruction.WebClient.Services;

public class BranchesController : BaseController
{
    private readonly IBranchesService _service;

    public BranchesController(IBranchesService service)
    {
        _service = service;
    }

    [Route("/{area}/branches", Name = "admin-branches")]
    public async Task<IActionResult> Index(SearchKeywordPagination model)
    {
        model.PageSize = 5;
        ViewBag.Keyword = model.Keyword;
        var blogCategories = await _service.GetPaginationAsync(model);
        return View(blogCategories);
    }

    [Route("/{area}/branches/action", Name = "admin-branches-action-add")]
    [Route("/{area}/branches/{id}/action", Name = "admin-branches-action")]
    public IActionResult Action()
    {
        return View();
    }

    [HttpPut]
    [Route("/{area}/branches/{id}/status")]
    public async Task<JsonResult> UpdateStatus(Guid id)
    {
        var result = await _service.ChangeStatus(id);
        return Json(result);
    }

    [HttpDelete]
    [Route("/{area}/branches/{id}/delete")]
    public async Task<JsonResult> Delete(Guid id)
    {
        var result = await _service.RemoveAsync(id);
        return Json(result);
    }

    [HttpPost]
    [Route("/{area}/branches/addorupdate")]
    public async Task<JsonResult> AddOrUpdate([FromForm] BranchesActionVM model)
    {
        var result = await _service.AddOrUpdateActionAsync(model);
        return Json(result);
    }

    [HttpGet]
    [Route("/{area}/branches/{id}")]
    public async Task<JsonResult> FindById(Guid id)
    {
        var result = await _service.FindByIdAsync(id);
        return Json(result);
    }
}