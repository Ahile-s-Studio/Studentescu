using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Studentescu.Models;

namespace Studentescu.Filters;

public class
    ProfileCompletionRequiredAttribute :
    IAsyncActionFilter,
    IPageFilter
{
    private readonly UserManager<ApplicationUser>
        _userManager;

    public ProfileCompletionRequiredAttribute(
        UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    // For Controllers (MVC)
    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        // Get the action name
        var actionName = context
            .ActionDescriptor.RouteValues["action"];


        // Get the controller name
        var controllerName = context
            .ActionDescriptor
            .RouteValues["controller"];

        Console.WriteLine($"action: {actionName}, controller: {controllerName}");

        if (actionName != null && controllerName != null &&
            controllerName.ToLower().Equals("profile") &&
            actionName.ToLower().Equals("completeprofile"))
        {
            await next();
            return;
        }

        var user = _userManager.GetUserAsync
        (context
            .HttpContext.User).Result;

        // Check if the user is authenticated and the profile is complete
        if (user is { IsProfileCompleted: false })
        {
            // Redirect to the profile completion page
            context.Result = new RedirectToActionResult("CompleteProfile", "Profile", null);
            return;
        }

        // Continue with the action execution if profile is complete or the user is not logged in
        await next();
    }

    // For Razor Pages
    public void OnPageHandlerSelected(
        PageHandlerSelectedContext context)
    {
    }


    public void OnPageHandlerExecuting(PageHandlerExecutingContext context)
    {
        var user = _userManager.GetUserAsync(context.HttpContext.User).Result;

        // Check if the page is "Logout" by comparing the route or path
        var isLogoutPage = context.HttpContext.Request.Path.StartsWithSegments("/Identity/Account/Logout");
        Console.WriteLine(context.HttpContext.Request.Path.ToString());

        // If it's the "Logout" page, continue to the next handler (do nothing)
        if (isLogoutPage)
        {
            return;
        }

        // If the profile is not complete, redirect to the "CompleteProfile" page
        if (user is { IsProfileCompleted: false })
        {
            context.Result = new RedirectResult("/Profile/CompleteProfile");
        }
    }


    public void OnPageHandlerExecuted(
        PageHandlerExecutedContext context)
    {
    }
}