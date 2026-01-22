using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CaoGiaConstruction.WebClient.Context.Entities
{
    [Table("HomeComponentConfigs")]
    public class HomeComponentConfig : EntityBase
    {
        [StringLength(255)]
        public string Name { get; set; }

        [StringLength(255)]
        public string ComponentKey { get; set; }

        public string? Javascript { get; set; }

        public int? SortOrder { get; set; }
    }
}
