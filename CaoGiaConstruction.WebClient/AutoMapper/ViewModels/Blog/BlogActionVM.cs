using System.ComponentModel.DataAnnotations;
using CaoGiaConstruction.WebClient.Context.Enums;

namespace CaoGiaConstruction.WebClient.AutoMapper.ViewModels
{
    public class BlogActionVM : BlogVM
    {
        public IFormFile File { get; set; }
    }
}