using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CaoGiaConstruction.WebClient.Context.Enums;
using CaoGiaConstruction.WebClient.Context.Interface;

namespace CaoGiaConstruction.WebClient.Context.Entities
{
    public class SlideCategory : EntityBase
    {
        [StringLength(255)]
        public string Title { get; set; }

        [StringLength(512)]
        public string Description { get; set; }

        public string? Content { get; set; }

        [StringLength(255)]
        public string Code { get; set; }

        [StringLength(255)]
        public string Avatar { get; set; }

        public int? SortOrder { get; set; }
    }
}