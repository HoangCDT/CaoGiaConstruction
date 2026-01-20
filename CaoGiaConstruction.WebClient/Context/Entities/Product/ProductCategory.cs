using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CaoGiaConstruction.WebClient.Context.Interface;

namespace CaoGiaConstruction.WebClient.Context.Entities
{
    public class ProductCategory : EntityBase, ISeoMetaData
    {
        [StringLength(512)]
        public string Title { get; set; }

        [StringLength(255)]
        public string Code { get; set; }

        [StringLength(255)]
        public string Avatar { get; set; }

        [StringLength(512)]
        public string Description { get; set; }

        public string? Content { get; set; }

        public int? SortOrder { get; set; }

        public Guid? ProductMainCategoryId { get; set; }

        [StringLength(60)]
        public string? SeoPageTitle { set; get; }

        [StringLength(60)]
        public string? SeoAlias { set; get; }

        [StringLength(60)]
        public string? SeoKeywords { set; get; }

        [StringLength(60)]
        public string? SeoDescription { get; set; }

        [ForeignKey(nameof(ProductMainCategoryId))]
        public virtual ProductMainCategory ProductMainCategory { get; set; }

        public virtual ICollection<Product> Products { get; set; }
        public virtual ICollection<ProductCategoryProperties> ProductCategoryProperties { get; set; }
    }
}