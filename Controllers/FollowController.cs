using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Studentescu.Data;
using Studentescu.Models;

namespace Studentescu.Controllers;

public class FollowController : BaseController
{
    // GET
    public FollowController(ILogger<HomeController> logger,
        ApplicationDbContext context, UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager) : base(logger, context,
        userManager, roleManager)
    {
    }

    public IActionResult Index()
    {
        return View();
    }
}