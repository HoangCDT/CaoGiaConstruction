using System.ComponentModel.DataAnnotations;

namespace CaoGiaConstruction.WebClient.Context.Interface
{
    public interface ISeoImageData
    {
        [StringLength(60)]
        public string? AltText { set; get; }

        [StringLength(60)]
        public string? ImageTitle { set; get; }

        [StringLength(60)]
        public string? Caption { set; get; }

        [StringLength(60)]
        public string? FileName { get; set; }
        public DateTime UploadDate { get; set; }
    }
}