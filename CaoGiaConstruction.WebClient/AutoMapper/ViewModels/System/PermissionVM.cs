namespace CaoGiaConstruction.WebClient.AutoMapper.ViewModels
{
    public class PermissionVM
    {
        public int? Id { get; set; }

        public int RoleId { get; set; }

        public int FunctionId { get; set; }

        public bool CanCreate { set; get; }

        public bool CanRead { set; get; }

        public bool CanUpdate { set; get; }

        public bool CanDelete { set; get; }

        public RoleVM AppRole { get; set; }
    }
}