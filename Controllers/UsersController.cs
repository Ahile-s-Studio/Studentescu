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


    public async Task<IActionResult> Edit(string? id)
    {
        try
        {
            var user = _dbContext.Users.First(u => u.Id == id);
            // var user = _dbContext.Users.First();

            var allRoles = GetAllRoles();

            var roleNames =
                await _userManager
                    .GetRolesAsync(user); // Lista de nume de roluri

            // Cautam ID-ul rolului in baza de date
            var userRole = _roleManager.Roles
                .Where(r => roleNames.Contains(r.Name))
                .Select(r => r.Id)
                .First(); // Selectam 1 singur rol

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
            Console.WriteLine();
            return NotFound();
        }
    }

    [HttpPost]
    public async Task<ActionResult> Edit(string id,
        ApplicationUser newData, [FromForm] string newRole)
    {
        var user = _dbContext.Users.Find(id);

        // user.AllRoles = GetAllRoles();


        if (!ModelState.IsValid || user == null)
        {
            return RedirectToAction("Index");
        }

        user.UserName = newData.UserName;
        user.Email = newData.Email;
        user.FirstName = newData.FirstName;
        user.LastName = newData.LastName;
        user.PhoneNumber = newData.PhoneNumber;


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
    public IActionResult Delete(string id)
    {
        var user = _dbContext.Users
            .Include("Followers")
            .Include("Followings")
            .Include("Posts")
            .Include("Comments")
            .Include("RequestsSent")
            .Include("RequestsReceived")
            .Include("GroupMemberships")
            .First(u => u.Id == id);

        // // Delete user comments
        // if (user.Comments.Count > 0)
        // {
        //     foreach (var comment in user.Comments)
        //     {
        //         db.Comments.Remove(comment);
        //     }
        // }
        //
        // // Delete user bookmarks
        // if (user.Bookmarks.Count > 0)
        // {
        //     foreach (var bookmark in user.Bookmarks)
        //     {
        //         db.Bookmarks.Remove(bookmark);
        //     }
        // }
        //
        // // Delete user articles
        // if (user.Articles.Count > 0)
        // {
        //     foreach (var article in user.Articles)
        //     {
        //         db.Articles.Remove(article);
        //     }
        // }

        _dbContext.Users.Remove(user);

        _dbContext.SaveChanges();
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