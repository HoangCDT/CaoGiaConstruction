using System.ComponentModel.DataAnnotations;

namespace CaoGiaConstruction.WebClient.Context.Entities
{
    public class TimeLine : EntityBase
    {
        [StringLength(255)]
        public string? EventDate { get; set; } // Ví dụ 10-07-2024

        [StringLength(255)]
        public string Title { get; set; } // e.g., "Khởi đầu khát vọng"

        [StringLength(255)]
        public string? Description { get; set; } // Mô tả

        public int SortOrder { get; set; }
    }
}