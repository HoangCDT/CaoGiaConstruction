using System.ComponentModel.DataAnnotations;

namespace CaoGiaConstruction.WebClient.Context.Entities
{
    public class Setting : EntityBase
    {
        [StringLength(50)]
        public string? PrimaryColor { get; set; } // Màu chính (primary color) (ví dụ: #007bff)

        [StringLength(50)]
        public string? SecondaryColor { get; set; } // Màu phụ (secondary color) (ví dụ: #6c757d)

        [StringLength(50)]
        public string? FooterBackgroundColor { get; set; } //  Màu nền của footer (ví dụ: #FFFFFF)

        [StringLength(50)]
        public string? HeaderMenuTextColor { get; set; } // Màu text menu ở header (ví dụ: #333333)

        [StringLength(50)]
        public string? HeaderMenuTextSelectedColor { get; set; } // Màu text menu khi selected ở header (ví dụ: #007bff)

        [StringLength(50)]
        public string? FooterSubTextColor { get; set; } // Màu sub text ở footer (ví dụ: #CCCCCC)

        [StringLength(50)]
        public string? HeaderMenuHoverColor { get; set; } // Màu khi hover menu ở header (ví dụ: #007bff)

        [StringLength(50)]
        public string? SubMenuTextColor { get; set; } // Màu text sub-menu (ví dụ: #333333)

        [StringLength(50)]
        public string? SubMenuBorderTopColor { get; set; } // Màu border top của sub-menu (ví dụ: #cdcdcd)

        [StringLength(100)]
        public string? FontFamily { get; set; } // Font-family cho toàn trang web

        [StringLength(10)]
        public string? FontSize { get; set; } // Font-size cho toàn trang web
    }
}