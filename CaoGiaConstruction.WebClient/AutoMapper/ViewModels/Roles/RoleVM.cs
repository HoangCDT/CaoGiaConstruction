namespace CaoGiaConstruction.WebClient.AutoMapper.ViewModels
{
    public class RoleVM
    {
        public int? Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public Guid? CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public Guid? ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string CreatedByName { get; set; }

        public string ModifiedByName { get; set; }

        public UserVM UserCreate { get; set; }

        public UserVM UserModified { get; set; }
    }
}