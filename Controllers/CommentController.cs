using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Studentescu.Data;
using Studentescu.Models;

namespace Studentescu.Controllers;

public class CommentController : BaseController
{
    // GET
    public CommentController(ILogger<HomeController> logger,
        ApplicationDbContext context, UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager) : base(logger, context,
        userManager, roleManager)
    {
    }

    public async Task<IActionResult> Delete(int commentId)
    {
        var userId = _userManager.GetUserId(User);
        var user = await _userManager.FindByIdAsync(userId);
        Console.WriteLine("I have attempted deleting a comment " + commentId.ToString());

        if (user == null)
        {
            return Redirect("/Identity/Account/Login");
        }

        var comment = _dbContext.Comments.FirstOrDefault(c => c.Id == commentId);

        if (comment == null)
        {
            Console.WriteLine("Inexistent comment");
            return NotFound();
        }

        if (comment.UserId != userId)
        {
            return Forbid();
        }

        _dbContext.Comments.Remove(comment);

        await _dbContext.SaveChangesAsync();

        return Ok();
    }
    public async Task<IActionResult> Create(int postId, string content)
    {
        var userId = _userManager.GetUserId(User);
        var user = await _userManager.FindByIdAsync(userId);
        Console.WriteLine("I have attempted creating a comment " + postId.ToString());

        if (user == null)
        {
            return Redirect("/Identity/Account/Login");
        }

        var post = _dbContext.Posts
            .Include(p => p.User)
            .FirstOrDefault(p => p.Id == postId);

        if (post == null)
        {
            Console.WriteLine("Inexistent post");
            return NotFound();
        }

        _dbContext.Comments.Add(new Comment { Content = content, Post = post, User = user, PostId = postId, UserId = userId });

        await _dbContext.SaveChangesAsync();


        return Ok();
    }

    public async Task<IActionResult> Index(int pageNumber, int pageSize, int postId)
    {
        var userId = _userManager.GetUserId(User);
        var user = await _userManager.FindByIdAsync(userId);
        Console.WriteLine("I have attempted showcase comments " + postId.ToString() + ' ' + pageNumber.ToString() + ' ' + pageSize.ToString());

        var post = _dbContext.Posts
            .Include(p => p.User)
            .FirstOrDefault(p => p.Id == postId);

        if (post == null)
        {
            return NotFound();
        }

        var IsGroupModerator = _dbContext.MemberInGroups.Any(m => m.UserId == userId && m.UserGroupId == post.GroupId && m.Role != GroupRole.Member);

        if (post == null)
        {
            Console.WriteLine("Inexistent post");
            return NotFound();
        }

        var comments = _dbContext.Comments
                                        .Include(c => c.User)
                                        .Where(c => c.PostId == postId).OrderBy(c => c.CommentedAt)
                                        .Select(c => new
                                        {
                                            c.Id,
                                            c.Content,
                                            c.CommentedAt,
                                            User = new
                                            {
                                                c.User.Id,
                                                c.User.UserName,
                                                c.User.ProfilePictureUrl,

                                            },
                                            isMyComment = c.User.Id == userId || (user != null && user.IsAdmin) || IsGroupModerator,

                                        })
                                        .Skip((pageNumber - 1) * pageSize).Take(pageSize)
                                        .ToList();

        return Ok(comments);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null)
        {
            return Unauthorized();
        }

        var comment = await _dbContext.Comments.FirstOrDefaultAsync(c => c.Id == id);
        if (comment == null)
        {
            return NotFound();
        }

        if (comment.UserId != userId)
        {
            return Forbid();
        }

        return View(comment);
    }


    [HttpPost]
    public async Task<IActionResult> Edit(int id, string content)
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null)
        {
            return Unauthorized();
        }

        var comment = await _dbContext.Comments.FirstOrDefaultAsync(c => c.Id == id);
        if (comment == null)
        {
            return NotFound();
        }

        if (comment.UserId != userId)
        {
            return Forbid();
        }

        comment.Content = content;

        _dbContext.Comments.Update(comment);
        await _dbContext.SaveChangesAsync();

        return RedirectToAction("Index", "Feed");
    }

}