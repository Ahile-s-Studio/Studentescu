using System.ComponentModel.DataAnnotations;

namespace Studentescu.Models;

public class UserGroup
{
    [Key] public int Id { get; set; }
    public required string Name { get; set; }
    
    public string GroupImageUrl { get; set; }
    
    public required string Description { get; set; }

    public required CategoryType Category { get; set; } = CategoryType.None;
    
    public  bool Active { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public required ICollection<MemberInGroup> Members { get; set; } = new List<MemberInGroup>();
    public required ICollection<Post> GroupPosts { get; set; } = new List<Post>();
}

public enum CategoryType
{
    None,
    Travel,
    Fashion,
    Gastronomy,
    HealthCare,
    Games,
    Study,
}