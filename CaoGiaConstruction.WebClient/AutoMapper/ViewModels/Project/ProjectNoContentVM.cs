using System.ComponentModel.DataAnnotations;

namespace CaoGiaConstruction.WebClient.AutoMapper.ViewModels
{
    public class ProjectNoContentVM : EntityBaseVM
    {
        [StringLength(512)]
        public string Title { get; set; }

        [StringLength(255)]
        public string Code { get; set; }
        public DateTime Date { get; set; }
        [StringLength(512)]
        public string Address { get; set; }
        public Guid ServiceId { get; set; }
        public virtual ServiceVM Service { get; set; }

        [StringLength(512)]
        public string Avatar { get; set; }
        [StringLength(512)]
        public string Description { get; set; }

        public string? ImageList { get; set; }

        public int? SortOrder { get; set; }

        [StringLength(60)]
        public string? SeoPageTitle { set; get; }

        [StringLength(60)]
        public string? SeoAlias { set; get; }

        [StringLength(60)]
        public string? SeoKeywords { set; get; }

        [StringLength(60)]
        public string? SeoDescription { get; set; }
        public bool? HomeFlag { set; get; }
        public bool? HotFlag { set; get; }
    }
}
