using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Studentescu.Data;
using Studentescu.Models;
using Studentescu.ViewModels;

namespace Studentescu.Controllers;

public class FeedController : BaseController
{
    // GET
    public FeedController(ILogger<HomeController> logger,
        ApplicationDbContext context, UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager) : base(logger, context,
        userManager, roleManager)
    {
    }

    public async Task<IActionResult> Index()
    {
        const int PageSize = 5;
        var pageNumber = 1;

        var userId = _userManager.GetUserId(User);
        var user = await _userManager.FindByIdAsync(userId);

        var posts = await _dbContext.Posts
            .Include(p => p.User)
            .Include(p => p.Comments)
            .Include(p => p.Likes)
            .Where(p => p.User.Public && p.GroupId == null)
            .OrderByDescending(p => p.CreatedAt)
            .Skip((pageNumber - 1) * PageSize)
            .Take(PageSize)
            .Select(p => new PostViewModel
            {
                Post = p,
                IsLiked = p.Likes.Any(l => l.UserId == userId),
                IsMyPost = p.UserId == userId || (user != null && user.IsAdmin)
            })
            .ToListAsync();


        return View(posts);
    }

    [HttpGet]
    public async Task<IActionResult> LoadMore(int pageNumber = 1, int pageSize = 5)
    {
        pageNumber = Math.Max(pageNumber, 1);

        var userId = _userManager.GetUserId(User);
        var user = await _userManager.FindByIdAsync(userId);

        var posts = await _dbContext.Posts
            .Include(p => p.User)
            .Include(p => p.Comments)
            .Include(p => p.Likes)
            .Where(p => p.User.Public && p.GroupId == null)
            .OrderByDescending(p => p.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new PostViewModel
            {
                Post = p,
                IsLiked = p.Likes.Any(l => l.UserId == userId),
                IsMyPost = p.UserId == userId || (user != null && user.IsAdmin)
            })
            .ToListAsync();

        if (!posts.Any())
        {
            return NoContent();
        }

        return PartialView("_PostListPartial", posts);
    }
}