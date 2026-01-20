using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CaoGiaConstruction.WebClient.Context.Entities
{
    public class Weight : EntityBase
    {
        [StringLength(512)]
        public string Title { get; set; }

        [StringLength(255)]
        public string Code { get; set; }

        [StringLength(512)]
        public string Description { get; set; }
    }

    public class ProductType : EntityBase
    {
        [StringLength(512)]
        public string Title { get; set; }

        [StringLength(255)]
        public string Code { get; set; }

        [StringLength(512)]
        public string Description { get; set; }
    }

    public class ProductPrice : EntityBase
    {
        public Guid ProductId { get; set; }
        public Guid WeightId { get; set; }
        public Guid ProductTypeId { get; set; }
        public int Price { get; set; }
        public int OldPrice { get; set; }

        [ForeignKey(nameof(WeightId))]
        public virtual Weight Weight { get; set; }

        [ForeignKey(nameof(ProductTypeId))]
        public virtual ProductType ProductType { get; set; }

        [ForeignKey(nameof(ProductId))]
        public virtual Product Product { get; set; }
    }
}