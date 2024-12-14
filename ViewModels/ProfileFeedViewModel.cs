using Studentescu.Models;

namespace Studentescu.ViewModels;

public class ProfileFeedViewModel
{
    public ApplicationUser User { get; set; }
    public List<PostViewModel> Posts { get; set; }
}
