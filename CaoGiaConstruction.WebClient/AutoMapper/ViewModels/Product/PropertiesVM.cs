using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CaoGiaConstruction.WebClient.AutoMapper.ViewModels
{
    public class PropertiesVM : EntityBaseVM
    {
        [StringLength(512)]
        public string Title { get; set; }

        [StringLength(255)]
        public string Code { get; set; }

        [StringLength(512)]
        public string Description { get; set; }
        public int DataType { get; set; }
        public virtual ProductCategoryVM ProductCategory { get; set; }

    }

    public class ProductCategoryPropertiesVM : EntityBaseVM // Cấu hình Property cho Catagory
    {
        public Guid ProductCategoryId { get; set; }
        public Guid PropertyId { get; set; }
    }

    public class ProductPropertiesVM : EntityBaseVM // Khai giá trị thuộc tính từng sản phẩm theo Catagory
    {
        public Guid ProductId { get; set; }
        public Guid PropertyId { get; set; }

        [StringLength(512)]
        public string Value { get; set; }

        [StringLength(512)]
        public string Description { get; set; }

        [ForeignKey(nameof(ProductId))]
        public virtual ProductVM Product { get; set; }

        [ForeignKey(nameof(PropertyId))]
        public virtual PropertiesVM Properties { get; set; }
    }
}