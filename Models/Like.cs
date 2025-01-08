using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Studentescu.Models;

public class Like
{
    [Key] public int Id { get; set; }

    [ForeignKey("User")] public required string UserId { get; set; }

    public ApplicationUser User { get; set; }

    [ForeignKey("Post")] public required int PostId { get; set; }

    public Post Post { get; set; }

    public DateTime LikedAt { get; set; } = DateTime.Now;
}