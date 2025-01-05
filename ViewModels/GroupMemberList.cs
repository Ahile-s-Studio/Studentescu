using Studentescu.Models;

namespace Studentescu.ViewModels;

public class MemberDescription
{
    public ApplicationUser user;
    public GroupRole role { get; set; }
}
public class GroupMemberList
{
    public List<MemberDescription> Members { get; set; }
    public bool IsAdmin { get; set; }
    public List<JoinRequest> JoinRequests { get; set; }

    public int GroupId { get; set; }
}