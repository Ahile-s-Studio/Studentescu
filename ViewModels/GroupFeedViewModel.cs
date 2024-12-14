using Studentescu.Models;

namespace Studentescu.ViewModels;

public class GroupFeedViewModel
{
    public UserGroup Group { get; set; }
    public bool IsJoined { get; set; }
    public List<PostViewModel> Posts { get; set; }
}