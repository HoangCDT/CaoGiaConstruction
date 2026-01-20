using System.ComponentModel.DataAnnotations;
using CaoGiaConstruction.WebClient.AutoMapper.ViewModels;

namespace CaoGiaConstruction.WebClient.AutoMapper.ViewModels

{
    public class VideoVM : EntityBaseVM
    {
        public string YouTubeId { get; set; }
        [StringLength(1024)]
        public string YouTubeURL { get; set; }
        [StringLength(512)]
        public string Title { get; set; }
        [StringLength(2048)]
        public string Description { get; set; }
        [StringLength(512)]
        public string ThumbnailURL { get; set; }
        [StringLength(2048)]
        public string Thumbnail { get; set; }
        public DateTime PublishedDate { get; set; }
        public string Duration { get; set; }

    }
}
