using CaoGiaConstruction.WebClient.AutoMapper.ViewModels;

namespace CaoGiaConstruction.WebClient.Dtos
{
    public class ProjectHomeDto
    {
        public List<ServiceNoContentVM>? Services { get; set; }
        public List<ProjectNoContentVM>? Projects { get; set; }
    }
}
