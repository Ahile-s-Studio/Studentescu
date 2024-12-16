using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Studentescu.Data;
using Studentescu.FormValidationModels;
using Studentescu.Models;

namespace Studentescu.Controllers;

public class PostController : BaseController
{
    // GET
    public PostController(ILogger<HomeController> logger,
        ApplicationDbContext context, UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager) : base(logger, context,
        userManager, roleManager)
    {
    }

    public async  Task<IActionResult> Show(int id)
    {
        var post = await _dbContext.Posts
            .Include(p=> p.User)
            .Include(p=> p.Comments)
            .Include(p=>p.Likes)
            .Where(p=> p.Id == id)
            .FirstOrDefaultAsync();
        
        return View(post);
    }
    
    
    public async Task<IActionResult> Create(int postDestination = -1)
    {
        var userId = _userManager.GetUserId(User);
        var user =await _userManager.FindByIdAsync(userId);
        if (userId == null)
        {
            return RedirectToAction("Login", "Account"); 
        }

        ViewBag.PostDestination = postDestination;
        Console.WriteLine( ViewBag.PostDestination + " content of the viewbag");
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(PostForm post)
    {
        Console.WriteLine("This is the post destination "+post.PostDestination);
        
        var userId = _userManager.GetUserId(User);
        if (userId == null)
        {
            return RedirectToAction("Login", "Account"); 
        }

        Console.WriteLine("In the action\n");
        if (ModelState.IsValid)
        {
            var user =await _userManager.FindByIdAsync(userId);
            var group = _dbContext.UserGroups
                .Include(g=>g.Members)
                .FirstOrDefault(g => g.Id == post.PostDestination);
            if (group != null)
            {
                if (!(group.Members.Any(inGroup => inGroup.UserId == userId && inGroup.UserGroupId == group.Id)))
                {
                    return Forbid();
                }
            }
            _dbContext.Posts.Add(new Post{UserId = userId, Content = post.Content, Title = post.Title, ContentType = post.ContentType, User = user, GroupId = group?.Id , UserGroup = group, CreatedAt = DateTime.Now});
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("Index", "Feed"); 
        }
        
        foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
        {
            Console.WriteLine(error.ErrorMessage);
            Console.WriteLine('\n');
        }

        return View(post); 
    }
    
    public async Task<IActionResult> Delete(int id)
    {
        var post = await _dbContext.Posts
            .FirstOrDefaultAsync(p => p.Id == id);

        if (post == null)
        {
            return NotFound();
        }

        var userId = _userManager.GetUserId(User);
        if (post.UserId != userId)
        {
            return Forbid(); 
        }

        return View(post); 
    }
    
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var post = await _dbContext.Posts
            .FirstOrDefaultAsync(p => p.Id == id);

        if (post == null)
        {
            return NotFound();
        }

        var userId = _userManager.GetUserId(User);
        if (post.UserId != userId)
        {
            return Forbid(); 
        }

        _dbContext.Posts.Remove(post);
        await _dbContext.SaveChangesAsync();

        return RedirectToAction("Index", "Feed"); 
    }

    public async Task<IActionResult> Like(int postId)
    {
        var userId = _userManager.GetUserId(User);
        var user = await _userManager.FindByIdAsync(userId);
        Console.WriteLine("I have attempted liked this post");

        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }
        
        var post = _dbContext.Posts
            .Include(p=> p.User)
            .FirstOrDefault(p => p.Id == postId);
        
        if (post == null)
        {
            return NotFound();
        }
        else
        {
            if (post.GroupId == null)
            {
                Console.WriteLine("this post is a public one");
                // A personal page post
                if (post.User.Public != false)
                {
                    Console.WriteLine("I liked it");
                    _dbContext.Likes.Add(new Like{UserId = userId, User = user, PostId = post.Id, Post = post});
                }
                else
                {
                    //Private Profile
                    if (_dbContext.Follows.Any(follow => follow.FollowerId == userId && follow.FolloweeId == post.UserId))
                    {
                        _dbContext.Likes.Add(new Like{UserId = userId, User = user, PostId = post.Id, Post = post});
                    }
                    else
                    {
                        return Forbid();
                    }
                }
               
            }
            else
            {
                Console.WriteLine("this post is a group one");
                // A group post
                var group = _dbContext.UserGroups.Include(g=>g.Members).FirstOrDefault(g => g.Id == post.GroupId);
                if (group.Members.Any(member => member.UserId == userId))
                {
                    Console.WriteLine("I liked it");
                    _dbContext.Likes.Add(new Like{UserId = userId, User = user, PostId = post.Id, Post = post});
                }
                else
                {
                    return Forbid();
                }
                
            }
        }
        
        await _dbContext.SaveChangesAsync();
        
        
        return Ok();
    }

}