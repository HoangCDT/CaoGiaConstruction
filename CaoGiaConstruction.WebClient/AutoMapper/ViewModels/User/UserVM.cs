using System.ComponentModel.DataAnnotations;

namespace CaoGiaConstruction.WebClient.AutoMapper.ViewModels
{
    public class UserVM : EntityBaseVM
    {
        [StringLength(255)]
        public string Username { get; set; }

        public string Password { get; set; }
        public string Email { get; set; }

        [StringLength(255)]
        public string Avatar { get; set; }

        [StringLength(255)]
        public string URLGg { get; set; }

        [StringLength(255)]
        public string URLFb { get; set; }

        [StringLength(50)]
        public string Zalo { get; set; }

        [Required]
        [StringLength(255)]
        public string FullName { get; set; }

        public DateTime? BirthDay { set; get; }

        [StringLength(15)]
        public string PhoneNumberOther { get; set; }

        [StringLength(255)]
        public string Address { get; set; }

        public int? Position { get; set; }
        public int? Gender { get; set; }
        public bool? SoftDelete { get; set; }
    }
}