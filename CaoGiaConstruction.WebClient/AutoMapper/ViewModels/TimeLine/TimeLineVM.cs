using System.ComponentModel.DataAnnotations;

namespace CaoGiaConstruction.WebClient.AutoMapper.ViewModels
{
    public class TimeLineVM : EntityBaseVM
    {
        [StringLength(255)]
        public string? EventDate { get; set; } // Ví dụ 10-07-2024

        [StringLength(255)]
        public string? Description { get; set; } // Mô tả
    }
}