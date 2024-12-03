using System.ComponentModel.DataAnnotations;

namespace Studentescu.Models;

public class UserGroup
{
    [Key] public int Id { get; set; }

    public required string Name { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public required ICollection<MemberInGroup> Members { get; set; } = new List<MemberInGroup>();
    public required ICollection<GroupMessage> GroupMessages { get; set; } = new List<GroupMessage>();
}