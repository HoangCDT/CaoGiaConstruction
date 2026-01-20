using System.ComponentModel.DataAnnotations;

namespace CaoGiaConstruction.WebClient.AutoMapper.ViewModels
{
    public class FeedbackVM : EntityBaseVM
    {
        [StringLength(255)]
        [Required]
        public string FullName { set; get; }

        [StringLength(255)]
        public string Avatar { get; set; }

        [StringLength(255)]
        public string Email { set; get; }

        [StringLength(512)]
        public string Description { set; get; }

        public string Content { set; get; }

        public int? SortOrder { get; set; }

        public bool? IsDeleted { get; set; }
    }
}