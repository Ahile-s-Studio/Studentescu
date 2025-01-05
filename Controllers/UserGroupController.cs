using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Studentescu.Data;
using Studentescu.Models;
using Studentescu.ViewModels;

namespace Studentescu.Controllers;

public class UserGroupController : BaseController
{
    // GET
    public UserGroupController(ILogger<HomeController> logger,
        ApplicationDbContext context, UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager) : base(logger, context,
        userManager, roleManager)
    {
    }

    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);
        var user = await _userManager.FindByIdAsync(userId);


        var groups = _dbContext.UserGroups
            .Include(p => p.Members)
            .Where(group => group.Active == true)
            .OrderBy(group => group.CreatedAt)
            .Take(10)
            .ToList();

        if (user != null && user.IsAdmin)
        {
            groups = _dbContext.UserGroups
                .Include(p => p.Members)
                .OrderBy(group => group.CreatedAt)
                .Take(10)
                .ToList();
        }

        var groupViews = new List<GroupIndexViewModel>();

        if (user == null)
        {
            foreach (var group in groups)
            {
                groupViews.Add(new GroupIndexViewModel { Group = group, IsJoined = false, IsAdmin = false, Status = "None", RequestId = -1 });
            }
        }
        else
        {
            foreach (var group in groups)
            {
                var relation = _dbContext.JoinRequests.FirstOrDefault(member => member.RequesterId == userId && member.GroupId == group.Id);
                string status = "None";
                var isJoined = _dbContext.MemberInGroups.Any(member => member.UserId == userId && member.UserGroupId == group.Id);
                Console.WriteLine(isJoined == true ? "Joined" : "Not Joined");
                if (relation != null && relation.Status == JoinRequestStatus.Rejected)
                {
                    status = "Rejected";
                }
                else if (relation != null && relation.Status == JoinRequestStatus.Pending)
                {
                    status = "Pending";
                }
                groupViews.Add(new GroupIndexViewModel { Group = group, IsJoined = isJoined, IsAdmin = user != null && user.IsAdmin, Status = status, RequestId = (relation is null) ? -1 : relation.Id });
            }
        }


        return View(groupViews);
    }
    public async Task<IActionResult> Show(int groupId = 1)
    {
        string userId = _userManager.GetUserId(User);
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var group = _dbContext.UserGroups.Include(g => g.Members).FirstOrDefault(group => group.Id == groupId);
        if ((group is null || group.Active is false) && !user.IsAdmin)
        {
            return NotFound();
        }

        var relationToGroup = _dbContext.MemberInGroups.FirstOrDefault(member => member.UserId == userId && member.UserGroupId == groupId);

        if (relationToGroup == null && !user.IsAdmin)
        {
            return Forbid();
        }

        bool isJoined = relationToGroup != null || user.IsAdmin;
        bool isModerator = relationToGroup != null && relationToGroup.Role != GroupRole.Member;
        bool isAdmin = relationToGroup != null && relationToGroup.Role == GroupRole.Admin || user.IsAdmin;
        var groupPosts = new List<PostViewModel>();

        if (isJoined)
        {
            groupPosts = _dbContext.Posts
                .Where(p => p.GroupId == groupId)
                .Include(p => p.User)
                .Include(p => p.Comments)
                .Include(p => p.Likes)
                .Select(p =>
                    new PostViewModel
                    {
                        Post = p,
                        IsLiked = p.Likes.Any(like => like.UserId == userId),
                        IsMyPost = (p.UserId == userId || isModerator || isAdmin)
                    }
                ).ToList();
        }
        Console.WriteLine("Here");
        return View(new GroupFeedViewModel { Group = group, IsJoined = isJoined, IsModerator = isModerator, IsAdmin = isAdmin, Posts = groupPosts });
    }

    public IActionResult Create()
    {


        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(UserGroup userGroup)
    {
        if (ModelState.IsValid)
        {
            Console.WriteLine("I am Creating a UserGroup");
            _dbContext.UserGroups.Add(userGroup);

            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);
            Console.WriteLine("Current user group id: " + userGroup.Id.ToString());
            _dbContext.MemberInGroups.Add(new MemberInGroup { User = user, UserId = userId, UserGroup = userGroup, UserGroupId = userGroup.Id, Role = GroupRole.Admin });

            await _dbContext.SaveChangesAsync();

            return RedirectToAction("Index", "UserGroup");
        }
        foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
        {
            Console.WriteLine(error.ErrorMessage);
        }

        return View(userGroup);
    }

    public async Task<IActionResult> JoinRequest(int groupId)
    {
        var group = _dbContext.UserGroups.FirstOrDefault(group => group.Id == groupId);
        if (group == null || group.Active == false)
        {
            Console.WriteLine("I am Joining a UserGroup");
            return Forbid();
        }
        string userId = _userManager.GetUserId(User);
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return Unauthorized();
        }

        if (_dbContext.MemberInGroups.FirstOrDefault(member =>
                member.UserId == userId && member.UserGroupId == groupId) != null)
        {
            Console.WriteLine("I am Joined a UserGroup2");
            return Forbid();
        }

        var admin = await _dbContext.MemberInGroups.Include(inGroup => inGroup.User).FirstOrDefaultAsync(member => member.UserGroupId == groupId && member.Role == GroupRole.Admin);

        if (admin is null || group.Active == false)
        {

            return NotFound();
        }

        _dbContext.JoinRequests.Add(new JoinRequest
        {
            GroupId = groupId,
            Status = JoinRequestStatus.Pending,
            RequesterId = userId,
            TargetId = admin.User.Id,
            CreatedAt = DateTime.Now,
            Group = group,
            Requester = user,
            Target = admin.User

        });

        await _dbContext.SaveChangesAsync();


        return Ok();
    }

    public async Task<IActionResult> CancelJoinRequest(int requestId)
    {

        var request = await _dbContext.JoinRequests.FirstOrDefaultAsync(r => r.Id == requestId);

        string userId = _userManager.GetUserId(User);
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null || request is null)
        {
            return NotFound();
        }

        if (request.RequesterId != user.Id || request.Status != JoinRequestStatus.Pending)
        {
            return Forbid();
        }

        _dbContext.JoinRequests.Remove(request);

        await _dbContext.SaveChangesAsync();

        return Ok();
    }
    public async Task<IActionResult> AcceptJoinRequest(int requestId, int groupId)
    {
        var request = _dbContext.JoinRequests.Include(j => j.Group).Include(j => j.Requester).FirstOrDefault(f => f.Id == requestId);
        string userId = _userManager.GetUserId(User);
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null || request is null)
        {
            return NotFound();
        }

        if (request.TargetId != userId)
        {
            return Forbid();
        }

        request.Status = JoinRequestStatus.Accepted;

        _dbContext.JoinRequests.Update(request);

        await _dbContext.SaveChangesAsync();

        _dbContext.MemberInGroups.Add(new MemberInGroup
        { User = request.Requester, UserId = request.RequesterId, UserGroup = request.Group, UserGroupId = request.GroupId, Role = GroupRole.Member });

        await _dbContext.SaveChangesAsync();

        return Redirect("/UserGroup/ShowMembers?groupId=" + groupId.ToString());
    }

    public async Task<IActionResult> DeclineJoinRequest(int requestId, int groupId)
    {
        var request = _dbContext.JoinRequests.Include(j => j.Group).FirstOrDefault(f => f.Id == requestId);
        string userId = _userManager.GetUserId(User);
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null || request is null)
        {
            return NotFound();
        }

        if (request.TargetId != userId)
        {
            return Forbid();
        }

        request.Status = JoinRequestStatus.Rejected;
        _dbContext.JoinRequests.Update(request);

        await _dbContext.SaveChangesAsync();

        return Redirect("/UserGroup/ShowMembers?groupId=" + groupId.ToString());
    }

    public async Task<IActionResult> Join(int groupId)
    {
        var group = _dbContext.UserGroups.FirstOrDefault(group => group.Id == groupId);
        if (group == null || group.Active == false)
        {
            return Forbid();
        }
        string userId = _userManager.GetUserId(User);
        var user = await _userManager.FindByIdAsync(userId);
        _dbContext.MemberInGroups.Add(new MemberInGroup
        { User = user, UserId = userId, UserGroup = group, UserGroupId = group.Id, Role = GroupRole.Member });

        await _dbContext.SaveChangesAsync();

        return RedirectToAction("Show", "UserGroup", new { groupId = group.Id });
    }

    public async Task<IActionResult> Leave(int groupId)
    {
        var group = _dbContext.UserGroups.FirstOrDefault(group => group.Id == groupId);
        if (group == null)
        {
            return NotFound();
        }
        string userId = _userManager.GetUserId(User);
        var relationToGroup = _dbContext.MemberInGroups.FirstOrDefault(member => member.UserId == userId && member.UserGroupId == groupId);

        if (relationToGroup != null && relationToGroup.Role == GroupRole.Admin)
        {
            group.Active = false;
        }

        _dbContext.MemberInGroups.Remove(relationToGroup);

        await _dbContext.SaveChangesAsync();

        return RedirectToAction("Index", "UserGroup");
    }

    public async Task<IActionResult> Delete(int groupId)
    {
        var group = await _dbContext.UserGroups.FirstOrDefaultAsync(g => g.Id == groupId);
        if (group == null)
        {
            return NotFound();
        }

        string userId = _userManager.GetUserId(User);
        var user = await _userManager.FindByIdAsync(userId);

        var relationToGroup = await _dbContext.MemberInGroups.FirstOrDefaultAsync(m => m.UserId == userId && m.UserGroupId == groupId);
        bool isUserAdmin = relationToGroup?.Role == GroupRole.Admin || (user?.IsAdmin ?? false);

        if (!isUserAdmin)
        {
            return Forbid();
        }

        using (var transaction = await _dbContext.Database.BeginTransactionAsync())
        {
            try
            {
                if (relationToGroup?.Role == GroupRole.Admin)
                {
                    // Deactivate the group (soft delete)
                    group.Active = false;
                }
                else
                {
                    // Hard delete: remove members, posts, comments, and join requests
                    var members = _dbContext.MemberInGroups.Where(m => m.UserGroupId == groupId);
                    _dbContext.MemberInGroups.RemoveRange(members);

                    var posts = _dbContext.Posts.Where(p => p.GroupId == groupId);
                    var likes = _dbContext.Likes.Where(c => posts.Select(p => p.Id).Contains(c.PostId));
                    var comments = _dbContext.Comments.Where(c => posts.Select(p => p.Id).Contains(c.PostId));
                    _dbContext.Comments.RemoveRange(comments);
                    _dbContext.Likes.RemoveRange(likes);
                    _dbContext.Posts.RemoveRange(posts);

                    var requests = _dbContext.JoinRequests.Where(r => r.GroupId == groupId);
                    _dbContext.JoinRequests.RemoveRange(requests);

                    _dbContext.UserGroups.Remove(group);
                }

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, "An error occurred while deleting the group.");
            }
        }

        return RedirectToAction("Index", "UserGroup");
    }

    public async Task<IActionResult> KickMember(int groupId, string userId)
    {
        var adminId = _userManager.GetUserId(User);
        var isAdmin = _dbContext.MemberInGroups.Any(m => m.UserId == adminId && m.UserGroupId == groupId && m.Role == GroupRole.Admin);

        if (!isAdmin)
        {
            return Forbid();
        }

        var userMembership = _dbContext.MemberInGroups.FirstOrDefault(m => m.UserId == userId && m.UserGroupId == groupId && m.UserId != adminId);

        if (userMembership == null)
        {
            return NotFound();
        }

        _dbContext.MemberInGroups.Remove(userMembership);

        var request = _dbContext.JoinRequests.FirstOrDefault(r => r.GroupId == groupId && r.RequesterId == userId && r.TargetId == adminId);

        if (request == null)
        {
            return NotFound();
        }

        request.Status = JoinRequestStatus.Rejected;

        _dbContext.JoinRequests.Update(request);

        await _dbContext.SaveChangesAsync();


        return Redirect("/UserGroup/ShowMembers?groupId=" + groupId.ToString());
    }

    public async Task<IActionResult> ShowMembers(int groupId)
    {
        var user = await _userManager.FindByIdAsync(_userManager.GetUserId(User));

        if (user is null)
        {
            return Unauthorized();
        }

        if (user.IsAdmin == false && !_dbContext.MemberInGroups.Any(m => m.UserId == user.Id && m.UserGroupId == groupId))
        {
            return Unauthorized();
        }

        var members = _dbContext.MemberInGroups.Include(members => members.User).Where(m => m.UserGroupId == groupId).Select(member => new MemberDescription { role = member.Role, user = member.User }).ToList();

        var requests = _dbContext.JoinRequests.Include(j => j.Requester).Where(r => r.GroupId == groupId && r.Status == JoinRequestStatus.Pending).ToList();


        bool isAdmin = _dbContext.MemberInGroups.Any(m => m.UserId == user.Id && m.UserGroupId == groupId && m.Role == GroupRole.Admin);

        if (!isAdmin)
        {
            return View(
                new GroupMemberList { Members = members, IsAdmin = isAdmin, JoinRequests = { }, GroupId = groupId });
        }

        return View(new GroupMemberList { Members = members, IsAdmin = isAdmin, JoinRequests = requests, GroupId = groupId });
    }

}