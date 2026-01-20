namespace CaoGiaConstruction.WebClient.AutoMapper.ViewModels
{
    public class ServiceGroupByCategoryVM
    {
        public ServiceCategoryVM? ServiceCategory { get; set; }

        public List<ServiceNoContentVM>? Service { get; set; }
    }
}
