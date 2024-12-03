using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Studentescu.Models;

public class Follow
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("Follower")]
    public required string FollowerId { get; set; }
    public required ApplicationUser Follower { get; set; }

    [ForeignKey("Followee")]
    public required string FolloweeId { get; set; }
    public required ApplicationUser Followee { get; set; }

    public DateTime FollowedAt { get; set; } = DateTime.Now;
}