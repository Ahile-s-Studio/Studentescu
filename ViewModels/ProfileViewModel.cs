using Studentescu.Models;

namespace Studentescu.ViewModels;

public class ProfileViewModel
{
    public ApplicationUser User { get; set; }
    public ApplicationUser CurrentUser { get; set; }
    public List<Post> UserPosts { get; set; }
    public List<ApplicationUser> RecommandedUsers { get; set; }
}