using Studentescu.Models;

namespace Studentescu.ViewModels;

public class PostViewModel
{
    public Post Post { get; set; }
    public bool IsLiked { get; set; }
    public bool IsSaved { get; set; }
}