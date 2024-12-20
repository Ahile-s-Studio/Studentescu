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

    public IActionResult Index()
    {
        var groups = _dbContext.UserGroups
            .Include(p=>p.Members)
            .OrderBy(group => group.CreatedAt)
            .Take(10)
            .ToList();
        var groupViews = new List<GroupIndexViewModel>();

        var userId = _userManager.GetUserId(User);
        
        foreach (var group in groups)
        {
            var relation = _dbContext.MemberInGroups.FirstOrDefault(member=> member.UserId == userId && member.UserGroupId == group.Id);
            bool isJoined = relation != null;
            groupViews.Add(new GroupIndexViewModel{Group = group, IsJoined = isJoined});
        }
        
        return View(groupViews);
    }
    public IActionResult Show(int groupId = 1)
    {
                
        
        var group = _dbContext.UserGroups.FirstOrDefault(group => group.Id == groupId);
        string userId = _userManager.GetUserId(User);
        var relationToGroup = _dbContext.MemberInGroups.FirstOrDefault(member => member.UserId == userId && member.UserGroupId == groupId);
        
        bool isJoined = relationToGroup != null;
        bool isModerator = relationToGroup.Role != GroupRole.Moderator;

        var groupPosts = new List<PostViewModel>();
        if (isJoined)
        {
            groupPosts =  _dbContext.Posts
                .Where(p => p.GroupId == groupId)
                .Include(p => p.User)
                .Include(p => p.Likes)
                .Select(p=> 
                    new PostViewModel
                        {Post = p, IsLiked = p.Likes.Any(like => like.UserId == userId), IsSaved = false}
                ).ToList();
        }
        return View(new GroupFeedViewModel{Group = group, IsJoined = isJoined, IsModerator = isModerator, Posts = groupPosts});
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
            Console.WriteLine("Current user group id: "+userGroup.Id.ToString());
            _dbContext.MemberInGroups.Add(new MemberInGroup{User = user, UserId = userId, UserGroup = userGroup, UserGroupId = userGroup.Id, Role = GroupRole.Admin});

            await _dbContext.SaveChangesAsync();
            
            return RedirectToAction("Index", "UserGroup"); 
        }
        foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
        {
            Console.WriteLine(error.ErrorMessage);
        }
        
        return View(userGroup);
    }

    public async Task<IActionResult> Join(int groupId)
    {
        var group = _dbContext.UserGroups.FirstOrDefault(group => group.Id == groupId);
        string userId = _userManager.GetUserId(User);
        var user = await _userManager.FindByIdAsync(userId);
        _dbContext.MemberInGroups.Add(new MemberInGroup
            { User = user, UserId = userId, UserGroup = group, UserGroupId = group.Id, Role = GroupRole.Member });
        
        await _dbContext.SaveChangesAsync();
        
        return RedirectToAction("Show", "UserGroup", new {groupId = group.Id});
    }
    
    public async Task<IActionResult> Leave(int groupId)
    {
        var group = _dbContext.UserGroups.FirstOrDefault(group => group.Id == groupId);
        if (group == null)
        {
            return NotFound();
        }
        string userId = _userManager.GetUserId(User);
        var relationToGroup =  _dbContext.MemberInGroups.FirstOrDefault(member => member.UserId == userId && member.UserGroupId == groupId);
        
        _dbContext.MemberInGroups.Remove(relationToGroup);
        if (relationToGroup.Role == GroupRole.Moderator)
        {
            group.Active = false; 
        }
        await _dbContext.SaveChangesAsync();
        
        return RedirectToAction("Index", "UserGroup");
    }

    public async Task<IActionResult> Delete(int groupId)
    {
        var group = _dbContext.UserGroups.FirstOrDefault(group => group.Id == groupId);
        if (group == null)
        {
            return NotFound();
        }
        
        string userId = _userManager.GetUserId(User);
        var user = await _userManager.FindByIdAsync(userId);
        var relationToGroup =  _dbContext.MemberInGroups.FirstOrDefault(member => member.UserId == userId && member.UserGroupId == groupId);

        if (relationToGroup.Role == GroupRole.Admin)
        {
            group.Active = false;
        }
        
        return RedirectToAction("Index", "UserGroup");
    }
    
    
}

