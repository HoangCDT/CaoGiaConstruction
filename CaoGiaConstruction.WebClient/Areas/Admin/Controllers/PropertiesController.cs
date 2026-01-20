using Microsoft.AspNetCore.Mvc;
using CaoGiaConstruction.WebClient.Areas.Admin.Controllers;
using CaoGiaConstruction.WebClient.Areas.Admin.Dtos;
using CaoGiaConstruction.WebClient.Context.Entities;
using CaoGiaConstruction.WebClient.Services;

public class PropertiesController : BaseController
{
    private readonly IPropertiesService _service;
    private readonly IDefineSystemService _defineSystemService;

    public PropertiesController(IPropertiesService service
    , IDefineSystemService defineSystemService
    )
    {
        _service = service;
        _defineSystemService = defineSystemService;
    }

    [Route("/{area}/properties", Name = "admin-properties")]
    public async Task<IActionResult> Index(SearchKeywordPagination model)
    {
        model.PageSize = 5;
        ViewBag.Keyword = model.Keyword;
        var blogCategories = await _service.GetPaginationAsync(model);

        return View(blogCategories);
    }

    [Route("/{area}/properties/action", Name = "admin-properties-action-add")]
    [Route("/{area}/properties/{id}/action", Name = "admin-properties-action")]
    public IActionResult Action()
    {
        return View();
    }

    [HttpPut]
    [Route("/{area}/properties/{id}/status")]
    public async Task<JsonResult> UpdateStatus(Guid id)
    {
        var result = await _service.ChangeStatus(id);
        return Json(result);
    }

    [HttpDelete]
    [Route("/{area}/properties/{id}/delete")]
    public async Task<JsonResult> Delete(Guid id)
    {
        var result = await _service.RemoveAsync(id);
        return Json(result);
    }

    [HttpPost]
    [Route("/{area}/properties/addorupdate")]
    public async Task<JsonResult> AddOrUpdate([FromBody] Properties model)
    {
        var result = await _service.AddOrUpdateAsync(model);
        return Json(result);
    }

    [HttpGet]
    [Route("/{area}/properties/{id}")]
    public async Task<JsonResult> FindById(Guid id)
    {
        var result = await _service.FindByIdAsync(id);
        return Json(result);
    }

    [HttpGet]
    [Route("/{area}/properties/validate-code/{id?}")]
    public JsonResult ValidateCode(string code, Guid? id)
    {
        var isPassed = _service.ValidateCode(code, id);
        return Json(isPassed);
    }
}