using System.ComponentModel.DataAnnotations;
using CaoGiaConstruction.WebClient.Context.Enums;

namespace CaoGiaConstruction.WebClient.AutoMapper.ViewModels
{
    public class AboutActionVM : AboutVM
    {
        public IFormFile FileLogoTop { get; set; }

        public IFormFile FileLogoBottom { get; set; }
    }
}