namespace DormInfrastructure.Models
{
    public class UserProfileViewModel
    {
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; } // 👈 add this
        public List<string> Roles { get; set; } = new();
    }


}
