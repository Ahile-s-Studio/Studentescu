using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Studentescu.Data;
using Studentescu.Models;
using Studentescu.ViewModels;

namespace Studentescu.Controllers;

public class ProfileController : BaseController
{
    private const int
        PageSize = 2; // Number of items per page

    public ProfileController(
        ILogger<HomeController> logger,
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager) : base(
        logger, context,
        userManager, roleManager)
    {
    }


    [HttpGet]
    public async Task<IActionResult> Search(
        [FromQuery] string query,
        [FromQuery] int page = 1)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return View(
                new PaginatedList<ApplicationUser>([],
                    0, page, PageSize));
        }

        var usersQuery = _userManager.Users
            .Where(u =>
                u.UserName.Contains(query) ||
                u.Email.Contains(query));

        var totalCount = await usersQuery.CountAsync();
        var users = await usersQuery
            .OrderBy(u => u.UserName)
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();

        var paginatedResult =
            new PaginatedList<ApplicationUser>(users,
                totalCount, page, PageSize);

        ViewData["query"] = query;

        return View(paginatedResult);
    }

    public async Task<IActionResult> Show(
        string? username)
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
                .ThenInclude(p => p.Likes)
                .First(u => u.UserName == username);
            var currentUser =
                await _userManager.GetUserAsync(User);

            if (user == null || currentUser == null)
            {
                return NotFound("username not found");
            }

            var posts = user.Posts
                .Where(post =>
                    post.GroupId == null &&
                    post.UserId == currentUser.Id);


            var viewUserPosts = posts.Select(post =>
                    new PostViewModel
                    {
                        Post = post,
                        IsLiked = post.Likes.Any(l =>
                            l.UserId ==
                            currentUser.Id),
                        // TODO implement
                        IsSaved = false
                    })
                .ToList();

            ProfileViewModel viewModel = new()
            {
                User = user,
                CurrentUser = currentUser,
                UserPosts = viewUserPosts,
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