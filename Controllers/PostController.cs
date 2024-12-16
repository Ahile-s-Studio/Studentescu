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
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(PostForm post)
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null)
        {
            return RedirectToAction("Login", "Account"); 
        }

        Console.WriteLine("In the action\n");
        if (ModelState.IsValid)
        {
            var user =await _userManager.FindByIdAsync(userId);
            var group = _dbContext.UserGroups.FirstOrDefault(g => g.Id == post.PostDestionation);
            _dbContext.Posts.Add(new Post{UserId = userId, Content = post.Content, Title = post.Title, ContentType = post.ContentType, User = user, GroupId = post.PostDestionation, UserGroup = group, CreatedAt = DateTime.Now});
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
    

}