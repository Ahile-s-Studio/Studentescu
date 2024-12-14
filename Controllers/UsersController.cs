using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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