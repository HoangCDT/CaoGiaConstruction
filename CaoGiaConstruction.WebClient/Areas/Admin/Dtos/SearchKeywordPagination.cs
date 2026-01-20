#nullable enable
using CaoGiaConstruction.Utilities;
using CaoGiaConstruction.WebClient.Context.Enums;

namespace CaoGiaConstruction.WebClient.Areas.Admin.Dtos
{
    public class SearchKeywordPagination : BasePagination
    {
        public string Keyword { get; set; }
        public StatusEnum? Status { get; set; }
        public string? Orderby { get; set; }
    }

    public class SearchProductCatKeywordPagination : SearchKeywordPagination
    {
        public Guid? ProductMainCategoryId { get; set; }
        public Guid? ProductCategoryId { get; set; }
    }


    public class SearchServiceCatKeywordPagination : SearchKeywordPagination
    {
        public Guid ServiceCategoryId { get; set; }
    }

    public class SearchBlogCatKeywordPagination : SearchKeywordPagination
    {
        public Guid BlogCategoryId { get; set; }

        public BlogTypeEnum? Type { get; set; }
    }

    public class SearchSlideKeywordPagination : SearchKeywordPagination
    {
        public Guid SlideCategoryId { get; set; }
    }
}