namespace CaoGiaConstruction.WebClient.AutoMapper.ViewModels
{
    public class BranchesVM : EntityBaseVM
    {

        public string? Title { get; set; } // Tên chi nhánh
        public string Code { get; set; } // Mã chi nhánh

        public string? Avatar { get; set; }

        public DateTime? FoundingDate { get; set; } // Ngày thành lập chi nhánh

        public string? Address { get; set; } // Địa chỉ chi nhánh.

        public string? PhoneNumber { get; set; } // Số điện thoại của chi nhánh.

        public string? Description { get; set; } // Mô tả chi nhánh

        public string? Latitude { get; set; } // Vĩ độ chi nhánh

        public string? Longitude { get; set; } // Kinh độ của chi nhánh.

        public string? MapIFrame { get; set; } // Nhúng link google maps

        public string? Content { get; set; }

        public string? SeoPageTitle { set; get; }

        public string? SeoAlias { set; get; }

        public string? SeoKeywords { set; get; }

        public string? SeoDescription { get; set; }

        public int? SortOrder { get; set; }
    }
}
