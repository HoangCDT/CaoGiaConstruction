using CaoGiaConstruction.WebClient.AutoMapper.ViewModels;

namespace CaoGiaConstruction.WebClient.Dtos
{
    public class ServiceHomeDto
    {
        public List<ServiceNoContentVM>? Services { get; set; }

        public ProjectNoContentVM? Projects { get; set; }
    }
}
