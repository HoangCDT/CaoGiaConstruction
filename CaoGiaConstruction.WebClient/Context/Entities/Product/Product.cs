using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CaoGiaConstruction.WebClient.Context.Interface;

namespace CaoGiaConstruction.WebClient.Context.Entities
{
    public class Product : EntityBase, ISeoMetaData
    {
        [StringLength(512)]
        public string Title { get; set; }

        [StringLength(255)]
        public string Code { get; set; }

        [StringLength(255)]
        public string Avatar { get; set; }

        [StringLength(4000)]
        public string ImageList { get; set; }

        [StringLength(512)]
        public string Description { get; set; }

        public string? Content { get; set; }

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

        [StringLength(60)]
        public string? SeoPageTitle { set; get; }

        [StringLength(60)]
        public string? SeoAlias { set; get; }

        [StringLength(60)]
        public string? SeoKeywords { set; get; }

        [StringLength(60)]
        public string? SeoDescription { get; set; }

        public Guid? ProductCategoryId { get; set; }

        public bool? HomeFlag { set; get; }

        public bool? HotFlag { set; get; }

        [ForeignKey(nameof(ProductCategoryId))]
        public virtual ProductCategory ProductCategory { get; set; }
        public ICollection<ProductProperties> ProductProperties { get; set; }
        public ICollection<ProductPrice> ProductPrices { get; set; }

    }
}