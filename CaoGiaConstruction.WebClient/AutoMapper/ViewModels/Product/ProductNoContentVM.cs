using System.ComponentModel.DataAnnotations;

namespace CaoGiaConstruction.WebClient.AutoMapper.ViewModels
{
    public class ProductNoContentVM : EntityBaseVM
    {
        [StringLength(512)]
        public string Title { get; set; }

        [StringLength(255)]
        public string Code { get; set; }

        [StringLength(255)]
        public string Avatar { get; set; }

        [StringLength(512)]
        public string ImageList { get; set; }

        [StringLength(512)]
        public string Description { get; set; }

        public int? SortOrder { get; set; }

        public double? Price { get; set; }

        public double? OldPrice { get; set; }

        [StringLength(255)]
        public string Promotions { get; set; } //Khuyến mãi

        [StringLength(255)]
        public string WarrantyPolicy { get; set; } //Chính sách bảo hành

        [StringLength(255)]
        public string Specifications { get; set; } //Quy cách

        [StringLength(255)]
        public string DosageForm { get; set; } //Dạng bào chế

        [StringLength(255)]
        public string Brand { get; set; } //Thương hiệu

        [StringLength(255)]
        public string Producer { get; set; } //Nhà sản xuất

        [StringLength(255)]
        public string Origin { get; set; } //Xuất xứ

        public string Accessories { get; set; } // Phụ kiện

        [StringLength(255)]
        public string Unit { get; set; } //Đơn vị tính

        public int? Quantity { get; set; }

        [StringLength(255)]
        public string SourcePage { get; set; }

        [StringLength(255)]
        public string SourceLink { get; set; }

        public int? ViewTime { get; set; }

        public bool? HomeFlag { set; get; }

        public bool? HotFlag { set; get; }


        public Guid ProductCategoryId { get; set; }

        public ProductCategoryVM ProductCategory { get; set; }
        public ICollection<ProductPropertiesVM> ProductProperties { get; set; }
        public ICollection<ProductPriceVM> ProductPrices { get; set; }
    }
}
