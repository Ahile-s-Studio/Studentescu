using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Studentescu.Data;
using Studentescu.Models;
using Studentescu.ViewModels;

namespace Studentescu.Controllers;

public class ProfileController : BaseController
{
    public ProfileController(ILogger<HomeController> logger,
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager) : base(logger, context,
        userManager, roleManager)
    {
    }

    public async Task<IActionResult> Show(string? username)
    {
        if (username == null)
        {
            return NotFound();
        }

        var user = _dbContext.Users
            .Include("Posts")
            .First(u => u.UserName == username);

        if (user == null)
        {
            return NotFound();
        }

        var posts = user.Posts.ToList();
        ProfileViewModel viewModel = new()
        {
            User = user,
            UserPosts = posts,
            RecommandedUsers = []
        };

        return View(viewModel);
    }
}