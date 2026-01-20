using System.ComponentModel.DataAnnotations;
using CaoGiaConstruction.WebClient.Context.Enums;

namespace CaoGiaConstruction.WebClient.Context.Entities
{
    public partial class BlogCategory : EntityBase
    {
        [StringLength(512)]
        public string Code { get; set; }

        [StringLength(512)]
        public string Title { get; set; }

        [StringLength(512)]
        public string Avatar { get; set; }

        [StringLength(512)]
        public string Description { get; set; }

        public int? SortOrder { get; set; }

        public BlogTypeEnum? Type { set; get; } // 0 - kiến thức, 1- Tin tức sự kiện, 2- Công thức

        public virtual ICollection<Blog> Blogs { get; set; }
    }
}