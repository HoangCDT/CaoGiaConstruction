using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CaoGiaConstruction.WebClient.Context.Enums;
using CaoGiaConstruction.WebClient.Context.Interface;

namespace CaoGiaConstruction.WebClient.Context.Entities
{
    public class Blog : EntityBase, ISeoMetaData
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

        public int? Position { get; set; }

        public bool? HomeFlag { set; get; }

        public bool? HotFlag { set; get; }

        public int? ViewTime { get; set; }

        public Guid? BlogCategoryId { get; set; }

        [StringLength(60)]
        public string? SeoPageTitle { set; get; }

        [StringLength(60)]
        public string? SeoAlias { set; get; }

        [StringLength(60)]
        public string? SeoKeywords { set; get; }

        [StringLength(60)]
        public string? SeoDescription { get; set; }

        public BlogTypeEnum? Type { set; get; } // 0 - kiến thức, 1- Tin tức sự kiện, 2- Công thức

        [ForeignKey(nameof(BlogCategoryId))]
        public virtual BlogCategory BlogCategory { get; set; }

    }
}