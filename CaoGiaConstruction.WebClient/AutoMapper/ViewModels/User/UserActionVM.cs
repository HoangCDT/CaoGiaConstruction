using System.ComponentModel.DataAnnotations;
using CaoGiaConstruction.WebClient.Context.Enums;

namespace CaoGiaConstruction.WebClient.AutoMapper.ViewModels
{
    public class UserActionVM
    {
        public Guid Id { get; set; }

        [StringLength(255)]
        public string Username { get; set; }

        public string Password { get; set; }
        public string NewPassword { get; set; }
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
        public string PhoneNumber { get; set; }

        [StringLength(15)]
        public string PhoneNumberOther { get; set; }

        [StringLength(255)]
        public string Address { get; set; }

        public int? Position { get; set; }
        public int? Gender { get; set; }
        public bool? SoftDelete { get; set; }
        public AccountStatus? Status { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public IFormFile File { get; set; }
    }
}