namespace CaoGiaConstruction.WebClient.AutoMapper.ViewModels
{
    public class HomeVM
    {
        public List<BlogNoContentVM>? HotBlogs { get; set; }
        public List<BlogNoContentVM> HomeBlogs { get; set; }
        public SlideVM? BannerBlog { get; set; }
    }
}
