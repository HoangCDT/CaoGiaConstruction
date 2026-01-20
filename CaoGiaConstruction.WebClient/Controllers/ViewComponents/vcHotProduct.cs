using Microsoft.AspNetCore.Mvc;
using CaoGiaConstruction.WebClient.Const;
using CaoGiaConstruction.WebClient.Services;

namespace CaoGiaConstruction.WebClient.Controllers.ViewComponents
{
    public class vcHotProduct : ViewComponent
    {
        private readonly IProductService _service;

        public vcHotProduct(IProductService service)
        {
            _service = service;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var data = await _service.GetProductsByMainCategoryCodeAsync(ProductMainCategoryCodeDefine.PRODUCT);

            return View(data);
        }
    }
}