namespace CaoGiaConstruction.WebClient.Dtos
{
    public abstract class CategoryWithCountDto<T>
    {
        public int? CountCategory { get; set; }

        public T Categories { get; set; }

    }
}
