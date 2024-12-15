using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Studentescu.Models;

public class Notification
{
    [Key] public int Id { get; set; }

    [Required] public required string Content { get; set; }

    [ForeignKey("User")] public required string SenderId { get; set; }

    public required ApplicationUser SenderUser { get; set; }

    [ForeignKey("User")] public required string ReceiverId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}