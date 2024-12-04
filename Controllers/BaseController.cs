using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Studentescu.Data;
using Studentescu.Models;

namespace Studentescu.Controllers;

public class BaseController : Controller
{
    protected readonly ApplicationDbContext dbContext;
    private readonly ILogger<HomeController> logger;
    private readonly RoleManager<IdentityRole> roleManager;
    private readonly UserManager<ApplicationUser> userManager;

    public BaseController(
        ILogger<HomeController> logger,
        ApplicationDbContext dbContext,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager
    )
    {
        this.logger = logger;
        this.dbContext = dbContext;
        this.userManager = userManager;
        this.roleManager = roleManager;
    }
}