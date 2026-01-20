using System.ComponentModel.DataAnnotations;

namespace CaoGiaConstruction.WebClient.AutoMapper.ViewModels
{
    public class ProjectVM : ProjectNoContentVM
    {
        public string? Content { get; set; }
        [StringLength(60)]
        public string? SeoPageTitle { set; get; }

        [StringLength(60)]
        public string? SeoAlias { set; get; }

        [StringLength(512)]
        public string? SeoKeywords { set; get; }

        [StringLength(512)]
        public string? SeoDescription { get; set; }
    }
}