using Studentescu.Models;

namespace Studentescu.ViewModels;

public class UsersShowViewModel
{
    public ApplicationUser User { get; set; }
    public ApplicationUser UserCurent { get; set; }
    public string UserRole { get; set; }
    public IList<string> Roles { get; set; }
}