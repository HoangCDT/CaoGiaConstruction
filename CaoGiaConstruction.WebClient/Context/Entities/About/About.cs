using System.ComponentModel.DataAnnotations;
using CaoGiaConstruction.WebClient.Context.Interface;

namespace CaoGiaConstruction.WebClient.Context.Entities
{
    public class About : EntityBase, ISeoMetaData
    {
        [StringLength(512)]
        public string AboutUs { get; set; }

        [StringLength(2048)]
        public string? Description { get; set; }

        public string Content { set; get; }

        [StringLength(255)]
        public string Address { get; set; }

        [StringLength(255)]
        public string Copyright { get; set; }

        [StringLength(255)]
        public string LogoTop { get; set; }

        [StringLength(255)]
        public string LogoBottom { get; set; }

        [StringLength(50)]
        public string PhoneNumber { get; set; }

        [StringLength(50)]
        public string PhoneNumberOther { get; set; }

        [StringLength(50)]
        public string Email { get; set; }

        [StringLength(1024)]
        public string? MapIFrame { get; set; }

        [StringLength(1024)]
        public string? TiktokIFrame { get; set; }

        [StringLength(1024)]
        public string? YouTubeIFrame { get; set; }

        [StringLength(255)]
        public string? FacebookUrl { get; set; }

        [StringLength(255)]
        public string? YoutubeUrl { get; set; }

        [StringLength(255)]
        public string? TiktokUsername { get; set; }

        [StringLength(60)]
        public string? SeoPageTitle { set; get; }

        [StringLength(60)]
        public string? SeoAlias { set; get; }

        [StringLength(60)]
        public string? SeoKeywords { set; get; }

        [StringLength(60)]
        public string? SeoDescription { get; set; }

    }
}