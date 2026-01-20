using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CaoGiaConstruction.WebClient.Context.Enums;
using CaoGiaConstruction.WebClient.Context.Interface;

namespace CaoGiaConstruction.WebClient.Context.Entities
{
    public class EntityBase : IEntityBase<Guid>, IUserTracking, IDateTracking, IActiveTracking
    {
        [Key]
        public Guid Id { get; set; }

        public StatusEnum? Status { get; set; } = StatusEnum.Active;

        public bool? IsDeleted { get; set; } = false;

        public Guid? CreatedBy { get; set; }

        public Guid? ModifiedBy { get; set; }

        [Column(TypeName = "timestamp with time zone")]
        public DateTime? CreatedDate { get; set; }

        [Column(TypeName = "timestamp with time zone")]
        public DateTime? ModifiedDate { get; set; }

        [ForeignKey(nameof(CreatedBy))]
        public virtual User UserCreated { get; set; }

        [ForeignKey(nameof(ModifiedBy))]
        public virtual User UserModified { get; set; }
    }
}