using System.ComponentModel.DataAnnotations;

namespace CaoGiaConstruction.WebClient.Context.Entities
{
    public class CoreValue : EntityBase
    {
        [StringLength(512)]
        public string Title { get; set; }

        public string Description { get; set; }

        [StringLength(100)]
        public string Code { get; set; } // Material Symbol icon name

        public int SortOrder { get; set; }
    }
}
