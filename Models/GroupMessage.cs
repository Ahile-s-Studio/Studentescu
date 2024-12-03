using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Studentescu.Models;

public class GroupMessage : Message
{
    [Required][ForeignKey("Group")] public required int UserGroupId { get; set; }
    public required UserGroup UserGroup { get; set; }
}