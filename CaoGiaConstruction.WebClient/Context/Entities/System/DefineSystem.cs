using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CaoGiaConstruction.WebClient.Context.Entities
{
    public class DefineSystem : EntityBase
    {
        [StringLength(64)]
        public string Code { get; set; }
        [StringLength(255)]
        public string Title { get; set; }
        [StringLength(255)]
        public string Value { get; set; }

        [StringLength(512)]
        public string Description { get; set; }

        public int? SortOrder { get; set; }

    }
}