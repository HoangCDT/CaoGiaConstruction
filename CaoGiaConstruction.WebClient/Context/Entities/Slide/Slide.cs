using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CaoGiaConstruction.WebClient.Context.Entities
{
    public class Slide : EntityBase
    {
        [StringLength(255)]
        public string Title { get; set; }

        [StringLength(512)]
        public string Description { get; set; }

        public string? Content { get; set; }

        [StringLength(255)]
        public string Code { get; set; }

        [StringLength(255)]
        public string Avatar { get; set; }

        [StringLength(512)]
        public string ImageList { get; set; }

        public int? SortOrder { get; set; }

        public Guid? SlideCategoryId { get; set; }

        [ForeignKey(nameof(SlideCategoryId))]
        public virtual SlideCategory SlideCategory { get; set; }

    }
}