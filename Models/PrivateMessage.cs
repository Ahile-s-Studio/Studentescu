using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Studentescu.Models;

public class PrivateMessage : Message
{
    [Required] [ForeignKey("Receiver")] public required string ReceiverId { get; set; }
    public required ApplicationUser Receiver { get; set; }
}