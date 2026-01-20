using System.ComponentModel.DataAnnotations;

namespace CaoGiaConstruction.WebClient.Context.Entities
{
    public class Setting : EntityBase
    {
        [StringLength(50)]
        public string? HeaderBackgroundColor { get; set; } //  Màu nền của header (ví dụ: #FFFFFF)

        [StringLength(50)]
        public string? FooterBackgroundColor { get; set; } //  Màu nền của header (ví dụ: #FFFFFF)

        [StringLength(100)]
        public string? FontFamily { get; set; } // Font-family cho toàn trang web

        [StringLength(10)]
        public string? FontSize { get; set; } // Font-family cho toàn trang web
    }
}