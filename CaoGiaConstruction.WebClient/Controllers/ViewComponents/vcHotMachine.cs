using Microsoft.AspNetCore.Mvc;
using CaoGiaConstruction.WebClient.Const;
using CaoGiaConstruction.WebClient.Services;

namespace CaoGiaConstruction.WebClient.Controllers.ViewComponents
{
    public class vcHotMachine : ViewComponent
    {
        private readonly IProductService _service;

        public vcHotMachine(IProductService service)
        {
            _service = service;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var data = await _service.GetProductsByMainCategoryCodeAsync(ProductMainCategoryCodeDefine.MACHINES);

            return View(data);
        }
    }
}