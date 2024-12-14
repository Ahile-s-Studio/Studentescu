using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Studentescu.Data;
using Studentescu.Models;
using Studentescu.ViewModels;

namespace Studentescu.Controllers;

[Authorize(Roles = "Admin")]
public class UsersController : BaseController
{
    // GET
    public UsersController(ILogger<HomeController> logger,
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager) : base(logger, context,
        userManager, roleManager)
    {
    }

    public IActionResult Index()
    {
        var users = _dbContext.Users.ToList();
        UsersViewModel viewModel = new()
        {
            Users = users
        };

        return View(viewModel);
    }
}