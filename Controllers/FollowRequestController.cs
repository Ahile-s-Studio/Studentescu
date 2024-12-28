using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Studentescu.Data;
using Studentescu.Models;
using Studentescu.Services;

namespace Studentescu.Controllers;

[Authorize(Roles = "Admin,User")]
public class FollowRequestController : BaseController
{
    private readonly FollowService _followService;

    // GET
    public FollowRequestController(ILogger<HomeController> logger,
        ApplicationDbContext context, UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager, FollowService followService) : base(logger, context,
        userManager, roleManager)
    {
        _followService = followService;
    }

    public async Task<IActionResult> Received()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        var userId = user.Id;

        var receivedRequests = await _dbContext.FollowRequests
            .Where(fr => fr.TargetId == userId && fr.Status == FollowRequestStatus.Pending)
            .Include(fr => fr.Requester)
            .ToListAsync();

        return View(receivedRequests);
    }

    public async Task<IActionResult> Sent()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        var userId = user.Id;

        var sentRequests = await _dbContext.FollowRequests
            .Where(fr => fr.RequesterId == userId)
            .Include(fr => fr.Target)
            .ToListAsync();

        return View(sentRequests);
    }

    [HttpPost]
    public async Task<IActionResult> Accept(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        var followRequest = await _followService.AcceptRequest(user.Id, id);
        if (!followRequest)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Received));
    }

    [HttpPost]
    public async Task<IActionResult> Reject(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        var followRequest = await _followService.RejectRequest(user.Id, id);
        if (!followRequest)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Received));
    }
}