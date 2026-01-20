namespace CaoGiaConstruction.WebClient.Context.Interface
{
    public interface IUserTracking
    {
        Guid? CreatedBy { get; set; }

        Guid? ModifiedBy { get; set; }
    }
}