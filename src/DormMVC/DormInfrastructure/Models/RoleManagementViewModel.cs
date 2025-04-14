namespace DormInfrastructure.Models
{
    public class RoleManagementViewModel
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public IList<string> CurrentRoles { get; set; }
        public List<string> AvailableRoles { get; set; }
        public string SelectedRole { get; set; }
    }

}
