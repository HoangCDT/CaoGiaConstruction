using System.ComponentModel.DataAnnotations;

namespace CaoGiaConstruction.WebClient.AutoMapper.ViewModels
{
    public class ProcessStepVM : EntityBaseVM
    {
        [StringLength(512)]
        public string Title { get; set; }

        public string Description { get; set; }

        public int? SortOrder { get; set; }
    }
}
