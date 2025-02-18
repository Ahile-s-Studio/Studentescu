using Studentescu.Models;

namespace Studentescu.ViewModels;

public class GroupFeedViewModel
{
    public UserGroup Group { get; set; }
    public bool IsJoined { get; set; }

    public bool IsModerator { get; set; }

    public bool IsAdmin { get; set; }
    public List<PostViewModel> Posts { get; set; }
}