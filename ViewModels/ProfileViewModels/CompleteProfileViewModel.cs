using System.ComponentModel.DataAnnotations;

namespace Studentescu.ViewModels;

public class CompleteProfileViewModel
{
    [Required(ErrorMessage =
        "First Name is required.")]
    public string? FirstName { get; set; }

    [Required(ErrorMessage = "Last Name is required.")]
    public string? LastName { get; set; }

    // [Required(ErrorMessage =
    //     "Profile Picture URL is required.")]
    public string? ProfilePictureUrl { get; set; }

    [Required(ErrorMessage = "Biography is required.")]
    public string? Biography { get; set; }

    public bool Public { get; set; } = false;
}