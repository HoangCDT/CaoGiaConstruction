using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CaoGiaConstruction.WebClient.Context.Entities
{
    [Table("Roles")]
    public class Role : IdentityRole<Guid>
    {
        [PersonalData]
        [StringLength(250)]
        public string Description { get; set; }

        [PersonalData]
        public Guid? CreatedBy { get; set; }

        [PersonalData]
        public DateTime? CreatedDate { get; set; }

        [PersonalData]
        public Guid? ModifiedBy { get; set; }

        [PersonalData]
        public DateTime? ModifiedDate { get; set; }

        [ForeignKey(nameof(CreatedBy))]
        public virtual User UserCreate { get; set; }

        [ForeignKey(nameof(ModifiedBy))]
        public virtual User UserModified { get; set; }
    }
}