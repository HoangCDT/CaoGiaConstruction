using System.ComponentModel.DataAnnotations;
using CaoGiaConstruction.WebClient.Context.Enums;

namespace CaoGiaConstruction.WebClient.AutoMapper.ViewModels
{
    public class SlideVM
    {
        public Guid Id { get; set; }

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

        public StatusEnum? Status { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public Guid? SlideCategoryId { get; set; }

        public Guid? CreatedBy { get; set; }

        public Guid? ModifiedBy { get; set; }
        public UserVM UserCreated { get; set; }

        public UserVM UserModified { get; set; }

        public SlideCategoryVM SlideCategory { get; set; }
    }
}