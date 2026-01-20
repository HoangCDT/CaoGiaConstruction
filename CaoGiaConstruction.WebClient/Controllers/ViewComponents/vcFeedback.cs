using Microsoft.AspNetCore.Mvc;
using CaoGiaConstruction.WebClient.Context.Enums;
using CaoGiaConstruction.WebClient.Services;

namespace CaoGiaConstruction.WebClient.Controllers.ViewComponents
{
    public class vcFeedback : ViewComponent
    {
        private readonly IFeedbackService _service;

        public vcFeedback(IFeedbackService service)
        {
            _service = service;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var data = await _service.GetAllAsync(x=>x.Status == StatusEnum.Active && x.IsDeleted != true);
            return View(data);
        }
    }
}