using Microsoft.AspNetCore.Mvc;
using CaoGiaConstruction.WebClient.AutoMapper.ViewModels;

namespace CaoGiaConstruction.WebClient.Controllers.ViewComponents
{
    public class vcThemeSettings : ViewComponent
    {
        public vcThemeSettings()
        {
        }

        public IViewComponentResult Invoke()
        {
            // Get Setting from ViewBag (set in _Layout.cshtml)
            var setting = ViewBag.Setting as SettingVM;
            
            return View(setting);
        }
    }
}
