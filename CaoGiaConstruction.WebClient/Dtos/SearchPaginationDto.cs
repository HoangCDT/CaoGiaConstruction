using CaoGiaConstruction.Utilities;

namespace CaoGiaConstruction.WebClient.Dtos
{
    public class SearchPaginationDto : BasePagination
    {
        public string? Keyword { get; set; }

        public string? Code { get; set; }
        
        public string? OrderBy { get; set; }
    }
}
