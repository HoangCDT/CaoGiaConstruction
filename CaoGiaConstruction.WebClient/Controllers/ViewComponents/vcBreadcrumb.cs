using Microsoft.AspNetCore.Mvc;
using CaoGiaConstruction.WebClient.Dtos;

namespace CaoGiaConstruction.WebClient.Controllers.ViewComponents
{
    public class vcBreadcrumb : ViewComponent
    {
        public vcBreadcrumb()
        {
        }
        public IViewComponentResult Invoke(List<BreadcrumbDto> items)
        {
            var activeItem = items.FirstOrDefault(i => i.IsActive);
            ViewBag.ActiveItemTitle = activeItem?.Title;

            return View(items);
        }
    }
}