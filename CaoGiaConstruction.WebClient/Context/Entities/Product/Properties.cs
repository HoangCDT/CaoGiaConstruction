using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CaoGiaConstruction.WebClient.Context.Entities
{
    public class Properties : EntityBase
    {
        [StringLength(512)]
        public string Title { get; set; }

        [StringLength(255)]
        public string Code { get; set; }

        [StringLength(512)]
        public string Description { get; set; }
        public int DataType { get; set; }

    }

    public class ProductCategoryProperties : EntityBase // Cấu hình Property cho Catagory
    {
        public Guid ProductCategoryId { get; set; }
        public Guid PropertyId { get; set; }

        [ForeignKey(nameof(ProductCategoryId))]
        public virtual ProductCategory ProductCategory { get; set; }

        [ForeignKey(nameof(PropertyId))]
        public virtual Properties Properties { get; set; }
    }

    public class ProductProperties : EntityBase // Khai giá trị thuộc tính từng sản phẩm theo Catagory
    {
        public Guid ProductId { get; set; }
        public Guid PropertyId { get; set; }

        [StringLength(512)]
        public string Value { get; set; }

        [StringLength(512)]
        public string Description { get; set; }

        [ForeignKey(nameof(ProductId))]
        public virtual Product Product { get; set; }

        [ForeignKey(nameof(PropertyId))]
        public virtual Properties Properties { get; set; }
    }
}