using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CaoGiaConstruction.WebClient.Context.Enums;

namespace CaoGiaConstruction.WebClient.AutoMapper.ViewModels
{
    public class BlogCategoryVM
    {
        public Guid? Id { get; set; }

        [StringLength(512)]
        public string Code { get; set; }

        [StringLength(512)]
        public string Title { get; set; }

        [StringLength(512)]
        public string Avatar { get; set; }

        [StringLength(512)]
        public string Description { get; set; }

        public int? SortOrder { get; set; }

        public StatusEnum? Status { get; set; }

        public Guid? CreatedBy { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? CreatedDate { get; set; }

        public Guid? ModifiedBy { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? ModifiedDate { get; set; }

        public UserVM UserCreated { get; set; }

        public UserVM UserModified { get; set; }

        public BlogTypeEnum? Type { set; get; } // 0 - kiến thức, 1- Tin tức sự kiện, 2- Công thức

    }
}