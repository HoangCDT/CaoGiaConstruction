using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CaoGiaConstruction.WebClient.AutoMapper.ViewModels
{
    public class WeightVM : EntityBaseVM
    {
        [StringLength(512)]
        public string Title { get; set; }

        [StringLength(255)]
        public string Code { get; set; }

        [StringLength(512)]
        public string Description { get; set; }
    }

    public class ProductTypeVM : EntityBaseVM
    {
        [StringLength(512)]
        public string Title { get; set; }

        [StringLength(255)]
        public string Code { get; set; }

        [StringLength(512)]
        public string Description { get; set; }
    }

    public class ProductPriceVM : EntityBaseVM
    {
        public Guid ProductId { get; set; }
        public Guid WeightId { get; set; }
        public Guid ProductTypeId { get; set; }
        public int Price { get; set; }
        public int OldPrice { get; set; }

        [ForeignKey(nameof(WeightId))]
        public virtual WeightVM Weight { get; set; }

        [ForeignKey(nameof(ProductTypeId))]
        public virtual ProductTypeVM ProductType { get; set; }

        [ForeignKey(nameof(ProductId))]
        public virtual ProductVM Product { get; set; }

    }
}