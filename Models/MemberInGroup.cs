using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Studentescu.Models;

public class MemberInGroup
{
    public required ApplicationUser User;

    [Required] [ForeignKey("User")] public required string UserId;
    [Key] public int Id { get; set; }


    [Required] [ForeignKey("UserGroup")] public required int UserGroupId { get; set; }
    public required UserGroup UserGroup { get; set; } // Navigation property

    public GroupRole Role { get; set; } = GroupRole.Member; // Default to Member

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

public enum GroupRole
{
    Member,
    Moderator,
    Admin
}