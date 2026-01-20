using System.ComponentModel.DataAnnotations;

namespace CaoGiaConstruction.WebClient.AutoMapper.ViewModels
{
    public class ProductCategoryVM : EntityBaseVM
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

        public Guid? ProductMainCategoryId { get; set; }

        public int? SortOrder { get; set; }

        public ProductMainCategoryVM ProductMainCategory { get; set; }

        public virtual ICollection<ProductCategoryPropertiesVM> ProductCategoryProperties { get; set; }
    }
}