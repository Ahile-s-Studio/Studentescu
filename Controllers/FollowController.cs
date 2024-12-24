using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Studentescu.Data;
using Studentescu.Models;

namespace Studentescu.Controllers;

[ApiController]
[Authorize(Roles = "Admin,User")]
public class FollowController : BaseController
{
    public FollowController(ILogger<HomeController> logger,
        ApplicationDbContext context, UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager) : base(logger, context,
        userManager, roleManager)
    {
    }


    [HttpGet("following")]
    public async Task<IActionResult> GetFollowing()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        var followingUsersIds = _dbContext.Follows.Where(f => f.FollowerId == user.Id)
            .Select(f => f.FolloweeId).ToList();

        var followingUsers = _dbContext.Users
            .Where(u => followingUsersIds.Contains(u.Id))
            .ToList();

        return Ok(followingUsers);
    }

    [HttpGet("followers")]
    public async Task<IActionResult> GetFollowers()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        var followerUserIds = _dbContext.Follows.Where(f => f.FolloweeId == user.Id)
            .Select(f => f.FollowerId).ToList();

        var followerUsers = _dbContext.Users
            .Where(u => followerUserIds.Contains(u.Id))
            .ToList();

        return Ok(followerUsers);
    }

    [HttpPost("follow/{userId}")]
    public async Task<IActionResult> FollowUser(string userId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        var targetUser = await _dbContext.Users.FindAsync(userId);
        if (targetUser == null)
        {
            return NotFound();
        }

        var followingIds = _dbContext.Follows.Where(f => f.FollowerId == user.Id).Select(f => f
            .FolloweeId);

        if (!followingIds.Contains(targetUser.Id))
        {
            var follow = new Follow { FollowerId = user.Id, FolloweeId = targetUser.Id };
            _dbContext.Follows.Add(follow);
            await _dbContext.SaveChangesAsync();
        }

        return Ok(new { message = "User followed successfully" });
    }

    [HttpPost("unfollow/{userId}")]
    public async Task<IActionResult> UnfollowUser(string userId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        var targetUser = await _dbContext.Users.FindAsync(userId);
        if (targetUser == null)
        {
            return NotFound();
        }

        var follow = _dbContext.Follows.FirstOrDefault(f =>
            f.FollowerId == user.Id && f.FolloweeId == targetUser.Id);

        if (follow != null)
        {
            _dbContext.Follows.Remove(follow);
            await _dbContext.SaveChangesAsync();
        }

        return Ok(new { message = "User unfollowed successfully" });
    }
}