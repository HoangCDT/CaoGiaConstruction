using CaoGiaConstruction.WebClient.AutoMapper.ViewModels;

namespace CaoGiaConstruction.WebClient.Dtos
{
    public class ProductCategoryDto
    {
        public string? MainTitle { get; set; }
        public IEnumerable<ProductCategoryVM>? Categories { get; set; }
    }
}