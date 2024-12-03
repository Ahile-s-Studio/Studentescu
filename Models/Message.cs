using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Studentescu.Models;

public abstract class Message
{
    [Key] public int Id { get; set; }

    [Required] [ForeignKey("Sender")] public required string SenderId { get; set; }

    public required ApplicationUser Sender { get; set; }

    [Required] public required string Content { get; set; }

    public DateTime SentAt { get; set; } = DateTime.Now;
}