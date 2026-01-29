using System.ComponentModel.DataAnnotations;

namespace CaoGiaConstruction.WebClient.AutoMapper.ViewModels
{
    public class TeamMemberActionVM
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(255)]
        public string FullName { get; set; }

        [Required]
        [StringLength(255)]
        public string Position { get; set; }

        [StringLength(512)]
        public string Avatar { get; set; }

        [StringLength(1024)]
        public string Quote { get; set; }

        public int SortOrder { get; set; }

        public bool IsFounder { get; set; }

        public int Status { get; set; }

        public IFormFile File { get; set; }
    }
}
