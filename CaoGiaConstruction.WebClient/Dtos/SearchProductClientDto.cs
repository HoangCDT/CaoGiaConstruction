using CaoGiaConstruction.Utilities;

namespace CaoGiaConstruction.WebClient.Dtos
{
    public class SearchProductClientDto : SearchPaginationDto
    {
        public double PriceFrom { get; set; }
        public double PriceTo { get; set; }
        public string? ProductMainCategoryCode { get; set; }
    }
}