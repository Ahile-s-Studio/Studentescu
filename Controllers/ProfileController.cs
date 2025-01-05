using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Studentescu.Data;
using Studentescu.Models;
using Studentescu.Services;
using Studentescu.ViewModels;

namespace Studentescu.Controllers;

public class ProfileController : BaseController
{
    private const int
        PageSize = 2; // Number of items per page

    private readonly FollowService _followService;

    public ProfileController(
        ILogger<HomeController> logger,
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager, FollowService followService) : base(
        logger, context,
        userManager, roleManager)
    {
        _followService = followService;
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
            var user = _dbContext.Users
                .Include(u => u.Followers)
                .Include(u => u.Following)
                .Include(u => u.Posts)
                .ThenInclude(p => p.Likes)
                .FirstOrDefault(u => u.UserName == username);
            var currentUser =
                await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound("username not found");
            }

            var posts = user.Posts
                .Where(post =>
                    post.GroupId == null &&
                    post.UserId == user.Id);


            var viewUserPosts = posts.Select(post =>
                    new PostViewModel
                    {
                        Post = post,
                        IsLiked = post.Likes.Any(l =>
                            l.UserId ==
                            currentUser?.Id),

                        IsMyPost = post.UserId == currentUser.Id,
                    })
                .ToList();

            ProfileViewModel viewModel = new()
            {
                User = user,
                CurrentUser = currentUser,
                UserPosts = viewUserPosts,
                RecommandedUsers = [],
                FollowerCount = user.Followers.Count,
                FollowingCount = user.Following.Count
            };

            return View(viewModel);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);

            return NotFound();
        }
    }

    [HttpGet]
    public async Task<IActionResult> FollowersAndFollowing(string username)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == username);
        if (user == null)
        {
            return NotFound("user not found");
        }


        if (user.Public == false && user.UserName != currentUser?.UserName)
        {
            if (currentUser == null)
            {
                return Redirect($"/Profile/Show/{username}");
            }

            if (await _followService.IsFollowing(currentUser.Id, user.Id) == null)
            {
                return Redirect($"/Profile/Show/{username}");
            }
        }


        var viewModel = _dbContext.Users
            .Include(u => u.Followers).ThenInclude(f => f.Follower)
            .Include(u => u.Following).ThenInclude(f => f.Followee)
            .First(u => u.Id == user.Id);


        return View(viewModel);
    }

    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> CompleteProfile()
    {
        var user =
            await _userManager.GetUserAsync(User);

        if (user == null)
        {
            return RedirectToRoute(
                "/Identity/Account/Login"); // Redirect to login if user is not found
        }


        var viewModel = new CompleteProfileViewModel
        {
            Biography = user.Biography,
            Public = user.Public,
            FirstName = user.FirstName,
            LastName = user.LastName
            // ProfilePictureUrl = user.ProfilePictureUrl
        };

        return View(viewModel);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> CompleteProfile(
        CompleteProfileViewModel model)
    {
        Console.WriteLine(JsonSerializer.Serialize(model));
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user =
            await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return
                RedirectToRoute(
                    "/Identity/Account/Login"); // Redirect to login if user is not found
        }

        user.FirstName = model.FirstName;
        user.LastName = model.LastName;
        user.Biography = model.Biography;
        user.Public = model.Public;
        user.IsProfileCompleted = true;

        var result =
            await _userManager.UpdateAsync(user);

        if (result.Succeeded)
        {
            return Redirect("/");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty,
                error.Description);
        }

        return View(model);
    }
}