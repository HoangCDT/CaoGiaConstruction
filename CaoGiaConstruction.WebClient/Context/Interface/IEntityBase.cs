namespace CaoGiaConstruction.WebClient.Context.Interface
{
    public interface IEntityBase <TKey>
    {
        TKey Id { get; set; }
    }
}