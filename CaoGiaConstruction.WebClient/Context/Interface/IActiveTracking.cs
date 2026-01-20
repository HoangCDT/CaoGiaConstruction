using CaoGiaConstruction.WebClient.Context.Enums;

namespace CaoGiaConstruction.WebClient.Context.Interface
{
    public interface IActiveTracking
    {
        StatusEnum? Status { get; set; }
    }
}