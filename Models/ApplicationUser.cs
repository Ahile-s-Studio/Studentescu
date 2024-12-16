using Microsoft.AspNetCore.Identity;

namespace Studentescu.Models;

// TODO Make username unique 
public class ApplicationUser : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string? Biography { get; set; }

    public bool Public { get; set; } = false;

    public ICollection<Follow> Following { get; set; } =
        new List<Follow>();

    public ICollection<Post> Posts { get; set; } = new List<Post>();

    public ICollection<Follow> Followers { get; set; } =
        new List<Follow>();

    public ICollection<Like> Likes { get; set; } = new List<Like>();

    public ICollection<Comment> Comments { get; set; } =
        new List<Comment>();

    public ICollection<FollowRequest> RequestsSent { get; set; } =
        new List<FollowRequest>();

    public ICollection<FollowRequest> RequestsReceived { get; set; } =
        new List<FollowRequest>();

    public ICollection<MemberInGroup> GroupMemberships { get; set; } =
        new List<MemberInGroup>();

    public ICollection<Message> MessagesSent { get; set; } =
        new List<Message>();

    public ICollection<PrivateMessage> PrivateMessagesReceived
    {
        get;
        set;
    } = new List<PrivateMessage>();
}