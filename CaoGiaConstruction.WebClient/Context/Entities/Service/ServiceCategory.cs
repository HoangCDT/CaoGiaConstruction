using System.ComponentModel.DataAnnotations;
using CaoGiaConstruction.WebClient.Context.Interface;

namespace CaoGiaConstruction.WebClient.Context.Entities
{
    public class ServiceCategory : EntityBase, ISeoMetaData
    {
        [StringLength(512)]
        public string Title { get; set; }

        [StringLength(255)]
        public string Code { get; set; }

        [StringLength(255)]
        public string Avatar { get; set; }

        [StringLength(512)]
        public string Description { get; set; }

        public string? Content { get; set; }

        public int? SortOrder { get; set; }

        [StringLength(60)]
        public string? SeoPageTitle { set; get; }

        [StringLength(60)]
        public string? SeoAlias { set; get; }

        [StringLength(60)]
        public string? SeoKeywords { set; get; }

        [StringLength(60)]
        public string? SeoDescription { get; set; }

        public virtual ICollection<Service> Services { get; set; }
    }
}