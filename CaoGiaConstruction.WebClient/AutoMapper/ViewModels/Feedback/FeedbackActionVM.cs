using System.ComponentModel.DataAnnotations;

namespace CaoGiaConstruction.WebClient.AutoMapper.ViewModels
{
    public class FeedbackActionVM : FeedbackVM
    {
        public IFormFile File { get; set; }
    }
}