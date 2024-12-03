using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Studentescu.Models;

public class Comment
{
    [Key] public int Id { get; set; }

    [Required] public required string Content { get; set; }

    [ForeignKey("User")] public required string UserId { get; set; }

    public required ApplicationUser User { get; set; }

    [ForeignKey("Post")] public int PostId { get; set; }

    public required Post Post { get; set; }

    public DateTime CommentedAt { get; set; } = DateTime.Now;
}