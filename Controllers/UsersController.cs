using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Studentescu.Data;
using Studentescu.Models;
using Studentescu.ViewModels;

namespace Studentescu.Controllers;

[Authorize(Roles = "Admin")]
public class UsersController : BaseController
{
    // GET
    public UsersController(ILogger<HomeController> logger,
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager) : base(logger, context,
        userManager, roleManager)
    {
    }


    public IActionResult Index()
    {
        var users = _dbContext.Users.ToList();
        UsersViewModel viewModel = new()
        {
            Users = users
        };

        return View(viewModel);
    }

    public async Task<ActionResult> Show(string id)
    {
        var user = await _dbContext.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        var roles = await _userManager.GetRolesAsync(user);
        var userCurent = await _userManager.GetUserAsync(User);

        UsersShowViewModel viewModel = new()
        {
            User = user,
            Roles = roles,
            UserCurent = userCurent!
        };

        return View(viewModel);
    }


    [HttpGet]
    public async Task<IActionResult> Edit(string? id)
    {
        try
        {
            var user = _dbContext.Users.First(u => u.Id == id);
            // var user = _dbContext.Users.First();

            var allRoles = GetAllRoles();

            var roleNames =
                await _userManager
                    .GetRolesAsync(user);

            var userRole = _roleManager.Roles
                .Where(r => roleNames.Contains(r.Name))
                .Select(r => r.Id)
                .First();

            UsersEditViewModel viewModel = new()
            {
                UserRole = userRole,
                User = user,
                AllRoles = allRoles,
                RoleNames = roleNames
            };
            return View(viewModel);
        }
        catch (Exception e)
        {
            _logger.LogError("User not found");
            return NotFound();
        }
    }

    [HttpPost]
    public async Task<ActionResult> Edit(string id,
        UsersEditViewModel newData, [FromForm] string newRole)
    {
        var user = _dbContext.Users.Find(id);

        // user.AllRoles = GetAllRoles();
        ModelState.Remove("AllRoles");
        ModelState.Remove("UserRole");
        ModelState.Remove("RoleNames");

        if (!ModelState.IsValid || user == null)
        {
            return RedirectToAction("Index");
        }

        _logger.LogInformation("User {0} edited",
            JsonSerializer.Serialize(newData,
                new JsonSerializerOptions
                    { WriteIndented = true }));

        user.UserName = newData.User.UserName;
        user.Email = newData.User.Email;
        user.FirstName = newData.User.FirstName;
        user.LastName = newData.User.LastName;
        user.PhoneNumber = newData.User.PhoneNumber;


        var roles = _dbContext.Roles.ToList();

        foreach (var role in roles)
        {
            if (role.Name != null)
            {
                await _userManager.RemoveFromRoleAsync(user,
                    role.Name);
            }
        }

        var roleName = await _roleManager.FindByIdAsync(newRole);
        if (roleName == null)
        {
            return RedirectToAction("Index");
        }

        await _userManager.AddToRoleAsync(user,
            roleName.ToString());

        _dbContext.SaveChanges();

        return RedirectToAction("Index");
    }


    [HttpPost]
    public async Task<IActionResult> Delete(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            // Return a bad request if the id is invalid
            return BadRequest("User ID is required.");
        }

        var user = await _dbContext.Users
            // .Include(u => u.Followers)
            // .Include(u => u.Following)
            // .Include(u => u.Posts)
            // .Include(u => u.Comments)
            // .Include(u => u.RequestsSent)
            // .Include(u => u.RequestsReceived)
            // .Include(u => u.GroupMemberships)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
        {
            return NotFound($"User with ID {id} not found.");
        }

        // if (user.Posts.Any())
        // {
        //     _dbContext.Posts.RemoveRange(user.Posts);
        // }
        //
        // // Example: Remove user's comments
        // if (user.Comments.Any())
        // {
        //     _dbContext.Comments.RemoveRange(user.Comments);
        // }

        // if (user.Followers.Any())
        // {
        //     _dbContext.Followers.RemoveRange(user.Followers);
        // }
        //
        // if (user.Followings.Any())
        // {
        //     _dbContext.Followings.RemoveRange(user.Followings);
        // }
        //
        // if (user.GroupMemberships.Any())
        // {
        //     _dbContext.GroupMemberships.RemoveRange(
        //         user.GroupMemberships);
        // }

        // if (user.RequestsSent.Any())
        // {
        //     _dbContext.RequestsSent.RemoveRange(user.RequestsSent);
        // }
        //
        // if (user.RequestsReceived.Any())
        // {
        //     _dbContext.RequestsReceived.RemoveRange(
        //         user.RequestsReceived);
        // }

        // Remove the user
        _dbContext.Users.Remove(user);

        // Save all changes
        await _dbContext.SaveChangesAsync();

        // Redirect to Index or show a success message
        return RedirectToAction("Index");
    }

    [NonAction]
    public IEnumerable<SelectListItem> GetAllRoles()
    {
        var selectList = new List<SelectListItem>();

        var roles = from role in _dbContext.Roles
            select role;

        foreach (var role in roles)
        {
            selectList.Add(new SelectListItem
            {
                Value = role.Id,
                Text = role.Name
            });
        }

        return selectList;
    }
}