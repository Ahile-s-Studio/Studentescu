using System.ComponentModel.DataAnnotations;

namespace Studentescu.FormValidationModels;

public class PostForm
{
    [Required][MaxLength(100)] public required string Title { get; set; }

    [Required] public required string Content { get; set; }
    
    [Required] public required string ContentType { get; set; }

}