using Microsoft.AspNetCore.Mvc.Rendering;
using Studentescu.Models;

namespace Studentescu.ViewModels;

public class UsersEditViewModel
{
    public ApplicationUser User { get; set; }
    public IEnumerable<SelectListItem> AllRoles { get; set; }
    public string UserRole { get; set; }
    public IList<string> RoleNames { get; set; }
}