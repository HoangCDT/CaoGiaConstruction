using System.ComponentModel.DataAnnotations;
using CaoGiaConstruction.WebClient.Context.Enums;

namespace CaoGiaConstruction.WebClient.AutoMapper.ViewModels
{
    public class SlideCategoryVM
    {
        public Guid? Id { get; set; }

        [StringLength(255)]
        public string Title { get; set; }

        [StringLength(512)]
        public string Description { get; set; }

        public string? Content { get; set; }

        [StringLength(255)]
        public string Code { get; set; }

        [StringLength(255)]
        public string Avatar { get; set; }

        public StatusEnum? Status { get; set; }

        public int? SortOrder { get; set; }

        public Guid? CreatedBy { get; set; }

        public Guid? ModifiedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public UserVM UserCreated { get; set; }

        public UserVM UserModified { get; set; }

        public ICollection<SlideVM> Slides { get; set; }
    }
}