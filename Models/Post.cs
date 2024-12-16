using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Studentescu.Models;

public class Post
{
    [Key] public int Id { get; set; }

    [Required][MaxLength(100)] public required string Title { get; set; }

    [Required] public required string Content { get; set; }
    
    [Required] public required string ContentType { get; set; }

    [ForeignKey("User")] public required string UserId { get; set; }

    [ForeignKey("UserGroup")] public int? GroupId { get; set; } = null;
    public required ApplicationUser User { get; set; }
    
    public UserGroup? UserGroup { get; set; } = null;
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public ICollection<Like> Likes { get; set; } = new List<Like>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}