using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CaoGiaConstruction.WebClient.Context.Interface;

namespace CaoGiaConstruction.WebClient.Context.Entities
{
    public class Branches : EntityBase, ISeoMetaData
    {
        [StringLength(255)]
        public string? Title { get; set; } // Tên chi nhánh
        public string Code { get; set; } // Mã chi nhánh

        [StringLength(512)]
        public string? Avatar { get; set; }

        [Column(TypeName = "timestamp with time zone")]
        public DateTime? FoundingDate { get; set; } // Ngày thành lập chi nhánh

        [StringLength(255)]
        public string? Address { get; set; } // Địa chỉ chi nhánh.

        [StringLength(255)]
        public string? PhoneNumber { get; set; } // Số điện thoại của chi nhánh.

        [StringLength(255)]
        public string? Description { get; set; } // Mô tả chi nhánh

        [StringLength(512)]
        public string? Latitude { get; set; } // Vĩ độ chi nhánh

        [StringLength(512)]
        public string? Longitude { get; set; } // Kinh độ của chi nhánh.

        [StringLength(1024)]
        public string? MapIFrame { get; set; } // Nhúng link google maps

        public string? Content { get; set; }

        [StringLength(60)]
        public string? SeoPageTitle { set; get; }

        [StringLength(60)]
        public string? SeoAlias { set; get; }

        [StringLength(60)]
        public string? SeoKeywords { set; get; }

        [StringLength(60)]
        public string? SeoDescription { get; set; }

        public int? SortOrder { get; set; }

    }
}