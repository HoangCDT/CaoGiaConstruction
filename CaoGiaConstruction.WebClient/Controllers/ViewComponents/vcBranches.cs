using Microsoft.AspNetCore.Mvc;
using CaoGiaConstruction.WebClient.Context.Enums;
using CaoGiaConstruction.WebClient.Services;

namespace CaoGiaConstruction.WebClient.Controllers.ViewComponents
{
    public class vcBranches : ViewComponent
    {
        private readonly IBranchesService _service;

        public vcBranches(IBranchesService service)
        {
            _service = service;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var branches = await _service.GetAllAsync(x=>x.IsDeleted != true && x.Status == StatusEnum.Active);
            return View(branches);
        }
    }
}