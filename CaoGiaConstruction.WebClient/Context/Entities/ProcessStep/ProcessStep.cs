using System.ComponentModel.DataAnnotations;

namespace CaoGiaConstruction.WebClient.Context.Entities
{
    public class ProcessStep : EntityBase
    {
        [StringLength(512)]
        public string Title { get; set; }

        public string Description { get; set; }

        public int? SortOrder { get; set; }
    }
}
