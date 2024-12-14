using Studentescu.Models;

namespace Studentescu.ViewModels;

public class ProfileViewModel
{
     ApplicationUser User {get; set;}
     List<Post> UserPosts {get; set;}
     List<ApplicationUser> RecommandedUsers {get; set;}
}