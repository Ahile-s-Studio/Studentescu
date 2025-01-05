using Studentescu.Models;

namespace Studentescu.ViewModels;

public class GroupIndexViewModel
{
    public bool IsJoined { get; set; }

    public string Status { get; set; }

    public int RequestId { get; set; }
    public bool IsAdmin { get; set; }
    public UserGroup Group { get; set; }
}