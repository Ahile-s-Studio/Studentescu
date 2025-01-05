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

    public async Task<IActionResult> Show(int id)
    {
        var post = await _dbContext.Posts
            .Include(p => p.User)
            .Include(p => p.Comments)
            .Include(p => p.Likes)
            .Where(p => p.Id == id)
            .FirstOrDefaultAsync();

        return View(post);
    }


    public async Task<IActionResult> Create(int postDestination = -1)
    {
        var userId = _userManager.GetUserId(User);
        var user = await _userManager.FindByIdAsync(userId);
        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        ViewBag.PostDestination = postDestination;
        Console.WriteLine(ViewBag.PostDestination + " content of the viewbag");
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(PostForm post)
    {
        Console.WriteLine("This is the post destination " + post.PostDestination);

        var userId = _userManager.GetUserId(User);
        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        Console.WriteLine("In the action\n");
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var group = _dbContext.UserGroups
                .Include(g => g.Members)
                .FirstOrDefault(g => g.Id == post.PostDestination);
            if (group != null)
            {
                if (!(group.Members.Any(inGroup => inGroup.UserId == userId && inGroup.UserGroupId == group.Id)))
                {
                    return Forbid();
                }
            }
            _dbContext.Posts.Add(new Post { UserId = userId, Content = post.Content, Title = post.Title, ContentType = post.ContentType, User = user, GroupId = group?.Id, UserGroup = group, CreatedAt = DateTime.Now });
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

    public async Task<IActionResult> Edit(int postId)
    {
        var userId = _userManager.GetUserId(User);
        var post = _dbContext.Posts.FirstOrDefault(p => p.Id == postId);
        if (userId == null)
        {
            return Unauthorized();
        }
        if (post == null)
        {
            return NotFound();
        }
        if (post.UserId != userId)
        {
            return Forbid();
        }

        var postViewModel = new PostForm { Content = post.Content, Title = post.Title, ContentType = post.ContentType, PostDestination = post.GroupId };
        ViewBag.PostDestination = post.GroupId;
        ViewBag.PostId = post.Id;
        return View(postViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(PostForm postForm, int postId)
    {
        Console.WriteLine("Trying to edit the post " + postForm.Content + " " + postForm.Title + " " + postForm.ContentType + " " + postId.ToString() + " " + postForm.PostDestination);
        var userId = _userManager.GetUserId(User);
        var post = _dbContext.Posts.FirstOrDefault(p => p.Id == postId);
        if (userId == null)
        {
            return Unauthorized();
        }
        if (post == null)
        {
            return NotFound();
        }
        if (post.UserId != userId)
        {
            return Forbid();
        }

        if (post.GroupId != postForm.PostDestination)
        {
            return Forbid();
        }

        post.Content = postForm.Content;
        post.ContentType = postForm.ContentType;
        post.Title = postForm.Title;

        _dbContext.Posts.Update(post);

        await _dbContext.SaveChangesAsync();

        return RedirectToAction("Index", "Feed");
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var post = await _dbContext.Posts
            .FirstOrDefaultAsync(p => p.Id == id);

        if (post == null)
        {
            return NotFound();
        }

        var userId = _userManager.GetUserId(User);
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
        {
            return Forbid();
        }

        if (user.IsAdmin == false && post.UserId != userId && post.GroupId == null)
        {
            return Forbid();
        }

        if (post.GroupId != null)
        {
            var hasRight = await _dbContext.MemberInGroups.FirstOrDefaultAsync(m => m.UserId == userId && m.UserGroupId == post.GroupId && (m.Role == GroupRole.Admin || m.Role == GroupRole.Moderator));
            if (hasRight == null)
            {
                return Forbid();
            }
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
        var user = await _userManager.FindByIdAsync(userId);

        var isGroupModerator = _dbContext.MemberInGroups.Any(m => m.UserId == userId && m.UserGroupId == post.GroupId && m.Role != GroupRole.Member);

        if (user != null && post.UserId != userId && !user.IsAdmin && !isGroupModerator)
        {
            return Forbid();
        }

        _dbContext.Posts.Remove(post);
        await _dbContext.SaveChangesAsync();

        return RedirectToAction("Index", "Feed");
    }



    [HttpPost]
    public async Task<IActionResult> Like(int postId)
    {
        var userId = _userManager.GetUserId(User);
        var user = await _userManager.FindByIdAsync(userId);
        Console.WriteLine("I have attempted like this post");

        if (user == null)
        {
            return Unauthorized();
        }

        var post = _dbContext.Posts
            .Include(p => p.User)
            .FirstOrDefault(p => p.Id == postId);

        if (post == null)
        {
            Console.WriteLine("Inexistent post");
            return NotFound();
        }
        else
        {
            var isLiked = _dbContext.Likes.Any(l => l.PostId == postId && l.UserId == userId);
            if (isLiked)
            {
                Console.WriteLine("Already liked this post");
                return Forbid();
            }
            if (post.GroupId == null)
            {
                Console.WriteLine("this post is a public one");
                // A personal page post
                if (post.User.Public != false)
                {
                    Console.WriteLine("I liked it");
                    _dbContext.Likes.Add(new Like { UserId = userId, User = user, PostId = post.Id, Post = post });
                }
                else
                {
                    //Private Profile
                    if (_dbContext.Follows.Any(follow => follow.FollowerId == userId && follow.FolloweeId == post.UserId))
                    {
                        _dbContext.Likes.Add(new Like { UserId = userId, User = user, PostId = post.Id, Post = post });
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
                var group = _dbContext.UserGroups.Include(g => g.Members).FirstOrDefault(g => g.Id == post.GroupId);
                if (group.Members.Any(member => member.UserId == userId))
                {
                    Console.WriteLine("I liked it");
                    _dbContext.Likes.Add(new Like { UserId = userId, User = user, PostId = post.Id, Post = post });
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

    [HttpPost]
    public async Task<IActionResult> Unlike(int postId)
    {
        var userId = _userManager.GetUserId(User);
        var user = await _userManager.FindByIdAsync(userId);
        Console.WriteLine("I have attempted unlike this post");

        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var post = _dbContext.Posts
            .Include(p => p.User)
            .FirstOrDefault(p => p.Id == postId);

        var like = _dbContext.Likes.FirstOrDefault(l => l.PostId == postId && l.UserId == userId);

        if (like != null)
        {
            _dbContext.Likes.Remove(like);
        }
        else
        {
            System.Console.WriteLine("ERROR HERE" + postId.ToString());
            return Forbid();
        }


        await _dbContext.SaveChangesAsync();

        return Ok();
    }

}