using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CaoGiaConstruction.WebClient.Context.Enums;

namespace CaoGiaConstruction.WebClient.Context.Entities
{
    [Table("Users")]
    public class User : IdentityUser<Guid>
    {
        [PersonalData]
        [StringLength(255)]
        public string Avatar { get; set; }

        [PersonalData]
        [StringLength(255)]
        public string URLGg { get; set; }

        [PersonalData]
        [StringLength(255)]
        public string URLFb { get; set; }

        [PersonalData]
        [StringLength(50)]
        public string Zalo { get; set; }

        [Required]
        [PersonalData]
        [StringLength(255)]
        public string FullName { get; set; }

        [PersonalData]
        public DateTime? BirthDay { set; get; }

        [PersonalData]
        [StringLength(15)]
        public string PhoneNumberOther { get; set; }

        [PersonalData]
        [StringLength(255)]
        public string Address { get; set; }

        [PersonalData]
        public int? Position { get; set; }

        [PersonalData]
        public int? Gender { get; set; }

        [PersonalData]
        public bool? SoftDelete { get; set; }

        [PersonalData]
        public AccountStatus? Status { get; set; }

        [PersonalData]
        public Guid? CreatedBy { get; set; }

        [PersonalData]
        [Column(TypeName = "timestamp with time zone")]
        public DateTime? CreatedDate { get; set; }

        [PersonalData]
        public Guid? ModifiedBy { get; set; }

        [PersonalData]
        [Column(TypeName = "timestamp with time zone")]
        public DateTime? ModifiedDate { get; set; }

        [ForeignKey(nameof(CreatedBy))]
        public virtual User UserCreate { get; set; }

        [ForeignKey(nameof(ModifiedBy))]
        public virtual User UserModified { get; set; }
    }
}