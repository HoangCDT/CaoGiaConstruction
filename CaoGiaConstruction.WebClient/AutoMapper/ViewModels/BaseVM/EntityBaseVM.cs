using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CaoGiaConstruction.WebClient.Context.Enums;

namespace CaoGiaConstruction.WebClient.AutoMapper.ViewModels
{
    public class EntityBaseVM
    {
        public Guid? Id { get; set; }

        public StatusEnum? Status { get; set; } = StatusEnum.Active;

        [StringLength(255)]
        public string Code { get; set; }

        public bool? IsDeleted { get; set; } = false;

        [Column(TypeName = "datetime2")]
        public DateTime? CreatedDate { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? ModifiedDate { get; set; }

        public Guid? CreatedBy { get; set; }

        public Guid? ModifiedBy { get; set; }

        public UserVM UserCreated { get; set; }

        public UserVM UserModified { get; set; }
    }
}