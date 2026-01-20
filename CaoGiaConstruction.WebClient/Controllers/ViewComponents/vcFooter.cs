using Microsoft.AspNetCore.Mvc;
using CaoGiaConstruction.WebClient.Dtos;

namespace CaoGiaConstruction.WebClient.Controllers.ViewComponents
{
    public class vcFooter : ViewComponent
    {
        public vcFooter()
        {
        }

        public IViewComponentResult Invoke(SystemDto model)
        {
            return View(model);
        }
    }
}