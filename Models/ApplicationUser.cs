namespace Studentescu.Models;
using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser
{
    // Add custom properties
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? ProfilePictureUrl { get; set; }

    // Collection of users that this user is following
    public ICollection<Follow> Following { get; set; } = new List<Follow>();

    // Collection of users who are following this user
    public ICollection<Follow> Followers { get; set; } = new List<Follow>();
}