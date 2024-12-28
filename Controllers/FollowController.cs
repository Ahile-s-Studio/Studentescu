using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Studentescu.Data;
using Studentescu.Models;
using Studentescu.Services;

namespace Studentescu.Controllers;

[ApiController]
[Authorize(Roles = "Admin,User")]
public class FollowController : BaseController
{
    private readonly FollowService _followService;

    public FollowController(ILogger<HomeController> logger,
        ApplicationDbContext context, UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager, FollowService followService) : base(logger, context,
        userManager, roleManager)
    {
        _followService = followService;
    }


    [HttpGet("following")]
    public async Task<IActionResult> GetFollowing()
    {
        var user = await _userManager.GetUserAsync(User);
        // if (user == null)
        // {
        //     return Unauthorized();
        // }

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

    [HttpGet("requests/sent")]
    public async Task<IActionResult> GetRequestsSent()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        var requestedUsersIds = _dbContext.FollowRequests.Where(f => f.RequesterId == user.Id)
            .Select(f => f.TargetId).ToList();

        var requestedUsers = _dbContext.Users
            .Where(u => requestedUsersIds.Contains(u.Id))
            .ToList();

        return Ok(requestedUsers);
    }

    [HttpGet("requests/received")]
    public async Task<IActionResult> GetRequestsReceived()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        var requestsOfUsersIds = _dbContext.FollowRequests.Where(f => f.TargetId == user.Id)
            .Select(f => f.TargetId).ToList();

        var requestsOfUsers = _dbContext.Users
            .Where(u => requestsOfUsersIds.Contains(u.Id))
            .ToList();

        return Ok(requestsOfUsers);
    }

    [HttpGet("requests/sent/pending")]
    public async Task<IActionResult> GetPendingRequestsSent()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        var requestedUsers = await _dbContext.FollowRequests
            .Where(fr => fr.RequesterId == user.Id && fr.Status == FollowRequestStatus.Pending)
            .Join(
                _dbContext.Users,
                fr => fr.TargetId, // Assuming TargetId is the requested user's ID
                u => u.Id,
                (fr, u) => u
            )
            .ToListAsync();


        return Ok(requestedUsers);
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

        if (targetUser.Id == user.Id)
        {
            return BadRequest(new { error = "You cannot follow yourself." });
        }

        var follow = await _followService.IsFollowing(user.Id, targetUser.Id);

        if (follow != null)
        {
            return Ok(new { message = "User followed successfully" });
        }

        // Daca e un profil public
        if (targetUser.Public)
        {
            follow = new Follow { FollowerId = user.Id, FolloweeId = targetUser.Id };
            _dbContext.Follows.Add(follow);
            await _dbContext.SaveChangesAsync();
            return Ok(new { message = "User followed successfully" });
        }

        var followRequest = await _followService.IsRequestSent(user.Id, targetUser.Id);

        // Trimitem follow request
        if (followRequest != null && followRequest.Status != FollowRequestStatus.Rejected)
        {
            return Ok(new { message = "User followed successfully" });
        }

        followRequest = new FollowRequest
        {
            RequesterId = user.Id,
            TargetId = targetUser.Id,
            Status = FollowRequestStatus.Pending
        };
        _dbContext.FollowRequests.Add(followRequest);
        await _dbContext.SaveChangesAsync();


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

        var followRequest = _dbContext.FollowRequests.FirstOrDefault(f =>
            f.RequesterId == user.Id && f.TargetId == targetUser.Id);

        if (followRequest != null)
        {
            _dbContext.FollowRequests.Remove(followRequest);
            await _dbContext.SaveChangesAsync();
        }

        return Ok(new { message = "User unfollowed successfully" });
    }
}