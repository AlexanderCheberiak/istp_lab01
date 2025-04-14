namespace DormInfrastructure.Models
{
    public class UserListItemViewModel
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public IList<string> Roles { get; set; }
    }
    public class UserListViewModel
    {
        public List<UserListItemViewModel> Users { get; set; }
    }

}
