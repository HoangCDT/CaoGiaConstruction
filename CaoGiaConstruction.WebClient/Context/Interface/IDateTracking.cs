using System.ComponentModel.DataAnnotations.Schema;

namespace CaoGiaConstruction.WebClient.Context.Interface
{
    public interface IDateTracking
    {
        [Column(TypeName = "datetime2")]
        DateTime? CreatedDate { set; get; }

        [Column(TypeName = "datetime2")]
        DateTime? ModifiedDate { set; get; }
    }
}