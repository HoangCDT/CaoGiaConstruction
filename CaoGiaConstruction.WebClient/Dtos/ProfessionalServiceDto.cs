using CaoGiaConstruction.WebClient.AutoMapper.ViewModels;

namespace CaoGiaConstruction.WebClient.Dtos
{
    public class ProfessionalServiceDto
    {
        public List<ServiceNoContentVM>? Services { get; set; }

        public ServiceNoContentVM? FeaturedProject { get; set; }

        public SettingVM? Setting { get; set; }
    }
}
