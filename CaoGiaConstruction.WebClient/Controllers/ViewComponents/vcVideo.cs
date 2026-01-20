using Microsoft.AspNetCore.Mvc;
using CaoGiaConstruction.WebClient.Const;
using CaoGiaConstruction.WebClient.Context.Enums;
using CaoGiaConstruction.WebClient.Services;

namespace CaoGiaConstruction.WebClient.Controllers.ViewComponents
{
    public class vcVideo : ViewComponent
    {
        private readonly IVideoService _service;

        public vcVideo(IVideoService service)
        {
            _service = service;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var data = await _service.GetAllAsync(x=>x.Status == StatusEnum.Active);

            return View(data);
        }
    }
}