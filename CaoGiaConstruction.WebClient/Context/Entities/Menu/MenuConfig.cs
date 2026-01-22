using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CaoGiaConstruction.WebClient.Context.Entities
{
    [Table("MenuConfigs")]
    public class MenuConfig : EntityBase
    {
        [StringLength(255)]
        public string Name { get; set; }

        [StringLength(255)]
        public string MenuKey { get; set; }

        public string MenuJson { get; set; }

        public int? SortOrder { get; set; }
    }
}
