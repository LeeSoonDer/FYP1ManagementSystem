namespace FYP1ManagementSystem.Models
{
    public class UserWithRolesViewModel
    {
        public ApplicationUser User { get; set; }
        public List<string> Roles { get; set; } = new();
    }

}
