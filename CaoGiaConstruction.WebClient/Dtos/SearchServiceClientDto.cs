using CaoGiaConstruction.Utilities;

namespace CaoGiaConstruction.WebClient.Dtos
{
    public class SearchServiceClientDto : SearchPaginationDto
    {
        public double PriceFrom { get; set; }
        public double PriceTo { get; set; }
    }
}