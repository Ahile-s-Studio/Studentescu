using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Studentescu.Data;
using Studentescu.Models;

namespace Studentescu.Controllers;

public class BaseController : Controller
{
    protected readonly ApplicationDbContext _dbContext;
    protected readonly ILogger<HomeController> _logger;
    protected readonly RoleManager<IdentityRole> _roleManager;
    protected readonly UserManager<ApplicationUser> _userManager;

    public BaseController(
        ILogger<HomeController> logger,
        ApplicationDbContext dbContext,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager
    )
    {
        this._logger = logger;
        this._dbContext = dbContext;
        this._userManager = userManager;
        this._roleManager = roleManager;
    }
}