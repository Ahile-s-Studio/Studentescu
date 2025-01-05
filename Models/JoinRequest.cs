using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Studentescu.Models;

public class JoinRequest
{
    [Key] public int Id { get; set; }

    [Required][ForeignKey("Requester")] public required string RequesterId { get; set; }
    public ApplicationUser Requester { get; set; }

    [Required][ForeignKey("Target")] public required string TargetId { get; set; }
    public ApplicationUser Target { get; set; }
    
    [Required][ForeignKey("Group")] public required int GroupId { get; set; }
    public UserGroup Group { get; set; }

    [Required] public required JoinRequestStatus Status { get; set; }
    

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }
}

// Enum for the status of the follow request
public enum JoinRequestStatus
{
    Pending,
    Accepted,
    Rejected
}