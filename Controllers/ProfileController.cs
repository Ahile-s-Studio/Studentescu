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
            return NotFound("username not found");
        }

        try
        {
            var pos = _dbContext.Posts.ToList();
            var user = _dbContext.Users
                 .Include(u => u.Posts)
                .First(u => u.UserName == username);
            var currentUser = await _userManager.GetUserAsync(User);

            if (user == null || currentUser == null)
            {
                return NotFound("username not found");
            }

            var posts = user.Posts
                .Where(post => post.GroupId == null && post.UserId == currentUser.Id)
                .ToList();
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