namespace CaoGiaConstruction.WebClient.AutoMapper.ViewModels
{
    public class ProductGroupByCategoryVM
    {
        public ProductCategoryVM? ProductCategory { get; set; }

        public List<ProductNoContentVM>? Products { get; set; }
    }   
}
