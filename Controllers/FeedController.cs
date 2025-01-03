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
    int PageSize { get; set; } = 10;
    public IActionResult Index(int pageNumber = 1)
    {
        pageNumber = Math.Max(pageNumber, 1);

        var totalPostsCount = _dbContext.Posts
            .Where(p => p.User.Public != false)
            .Count();

        var totalPages = (int)Math.Ceiling(totalPostsCount / (double)PageSize);

        var userId = _userManager.GetUserId(User);

        var posts = _dbContext.Posts
            .Include(p => p.User)
            .Include(p => p.Comments)
            .Include(p => p.Likes)
            .Where(p => p.User.Public != false && p.GroupId == null)
            .OrderByDescending(p => p.CreatedAt)
            .Skip((pageNumber - 1) * PageSize)
            .Take(PageSize)
            .Select(p => new PostViewModel
            {
                Post = p,
                IsLiked = p.Likes.Any(l => l.UserId == userId),
                IsSaved = false,
            })
            .ToList();


        return View(posts);
    }
}