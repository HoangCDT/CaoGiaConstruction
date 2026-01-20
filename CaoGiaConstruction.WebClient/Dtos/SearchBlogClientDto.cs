using CaoGiaConstruction.WebClient.Context.Enums;

namespace CaoGiaConstruction.WebClient.Dtos
{
    public class SearchBlogClientDto : SearchPaginationDto
    {
        public BlogTypeEnum? Type { set; get; } // 0 - kiến thức, 1- Tin tức sự kiện, 2- Công thức
    }
}