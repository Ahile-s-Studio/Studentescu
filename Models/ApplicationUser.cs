using Microsoft.AspNetCore.Identity;

namespace Studentescu.Models;

public class ApplicationUser : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? ProfilePictureUrl { get; set; }

    public ICollection<Follow> Following { get; set; } = new List<Follow>();
    public ICollection<Follow> Followers { get; set; } = new List<Follow>();
    public ICollection<Like> Likes { get; set; } = new List<Like>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<FollowRequest> RequestsSent { get; set; } = new List<FollowRequest>();
    public ICollection<FollowRequest> RequestsReceived { get; set; } = new List<FollowRequest>();
}