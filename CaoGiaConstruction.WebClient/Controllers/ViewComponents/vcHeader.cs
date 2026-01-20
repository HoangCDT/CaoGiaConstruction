using Microsoft.AspNetCore.Mvc;
using CaoGiaConstruction.WebClient.Dtos;

namespace CaoGiaConstruction.WebClient.Controllers.ViewComponents
{
    public class vcHeader : ViewComponent
    {
        public vcHeader()
        {
        }

        public IViewComponentResult Invoke(SystemDto model)
        {
            return View(model);
        }
    }
}