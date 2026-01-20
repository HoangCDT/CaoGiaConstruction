using System.ComponentModel.DataAnnotations;

namespace CaoGiaConstruction.WebClient.Context.Entities
{
    public class Contact : EntityBase
    {
        [StringLength(255)]
        public string FullName { get; set; }

        [StringLength(255)]
        public string Email { get; set; }

        [StringLength(50)]
        public string PhoneNumber { get; set; }

        [StringLength(512)]
        public string Title { get; set; }

        public string? Content { get; set; }

        public Guid? ProductId { get; set; }

        public virtual Product Product { get; set; }
    }
}