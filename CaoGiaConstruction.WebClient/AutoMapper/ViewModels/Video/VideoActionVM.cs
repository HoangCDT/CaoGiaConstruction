using System.ComponentModel.DataAnnotations;
using CaoGiaConstruction.WebClient.AutoMapper.ViewModels;

namespace CaoGiaConstruction.WebClient.AutoMapper.ViewModels

{
    public class VideoActionVM : VideoVM
    {
        public IFormFile? File { get; set; }
    }
}
