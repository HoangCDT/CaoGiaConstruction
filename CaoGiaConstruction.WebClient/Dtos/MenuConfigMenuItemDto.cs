namespace CaoGiaConstruction.WebClient.Dtos
{
    public class MenuConfigMenuItemDto
    {
        public string Title { get; set; }
        public string RouteName { get; set; }
        public string Url { get; set; }
        public List<MenuConfigMenuItemDto> Children { get; set; }
    }
}
