using System.ComponentModel.DataAnnotations;
using CaoGiaConstruction.WebClient.Context.Enums;

namespace CaoGiaConstruction.WebClient.AutoMapper.ViewModels
{
    public class BlogNoContentVM : EntityBaseVM
    {
        [StringLength(512)]
        public string Title { get; set; }

        [StringLength(255)]
        public string Code { get; set; }

        [StringLength(255)]
        public string Avatar { get; set; }

        [StringLength(512)]
        public string Description { get; set; }

        public int? Position { get; set; }

        public int? ViewTime { get; set; }

        public bool? HomeFlag { set; get; }

        public bool? HotFlag { set; get; }

        public Guid? BlogCategoryId { get; set; }

        public BlogTypeEnum? Type { set; get; } // 0 - kiến thức, 1- Tin tức sự kiện, 2- Công thức


        public BlogCategoryVM BlogCategory { get; set; }
    }
}
