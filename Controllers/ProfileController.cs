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

        try
        {
            var user = _dbContext.Users
                .Include("Posts")
                .First(u => u.UserName == username);
            var currentUser = await _userManager.GetUserAsync(User);

            if (user == null || currentUser == null)
            {
                return NotFound();
            }

            var posts = user.Posts.ToList();
            ProfileViewModel viewModel = new()
            {
                User = user,
                CurrentUser = currentUser,
                UserPosts = posts,
                RecommandedUsers = []
            };

            return View(viewModel);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);

            return NotFound();
        }
    }
}