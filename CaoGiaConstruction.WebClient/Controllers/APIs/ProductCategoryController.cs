using Microsoft.AspNetCore.Mvc;
using CaoGiaConstruction.WebClient.Services;

namespace CaoGiaConstruction.WebClient.Controllers.APIs
{
    [Route("api/")]
    [ApiController]
    public class ProductCategoryController : ControllerBase
    {
        private readonly IProductCategoryService _service;

        public ProductCategoryController(IProductCategoryService service)
        {
            _service = service;
        }

        /// <summary>
        /// Retrieve all product categories
        /// </summary>
        /// <returns></returns>
        
        [HttpGet("productCategories")]
        public async Task<IActionResult> GetAllAsync()
        {
            return Ok(await _service.GetAllAsync());
        }
    }
}
