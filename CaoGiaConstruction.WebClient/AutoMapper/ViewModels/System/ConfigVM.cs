using CaoGiaConstruction.WebClient.Context.Enums;

namespace CaoGiaConstruction.WebClient.AutoMapper.ViewModels
{
    public class ConfigVM
    {
        public Guid? Id { get; set; }
        public string Code { get; set; }
        public StatusEnum Status { get; set; } = StatusEnum.Active;
        public Guid? CreatedBy { get; set; }
        public UserVM UserCreated { get; set; }
        public Guid? ModifiedBy { get; set; }
        public string CreatedByName { get; set; }

        public string ModifiedByName { get; set; }
        public UserVM UserModified { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}