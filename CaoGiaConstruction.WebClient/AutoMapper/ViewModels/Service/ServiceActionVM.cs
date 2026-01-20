using System.ComponentModel.DataAnnotations;
using CaoGiaConstruction.WebClient.Context.Enums;

namespace CaoGiaConstruction.WebClient.AutoMapper.ViewModels
{
    public class ServiceActionVM : ServiceVM
    {
        public List<IFormFile> Files { get; set; }
        public IFormFile File { get; set; }
    }
}