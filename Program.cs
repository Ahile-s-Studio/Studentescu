using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Studentescu.Data;
using Studentescu.Filters;
using Studentescu.Models;
using Studentescu.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString =
    builder.Configuration.GetConnectionString(
        "DefaultConnection") ??
    throw new InvalidOperationException(
        "Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(
    options =>
        options.UseMySql(connectionString,
            new MySqlServerVersion(
                new Version(8, 0, 33))));

builder.Services
    .AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(
        options =>
            options.SignIn.RequireConfirmedAccount =
                true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews(options =>
{
    options.Filters
        .Add<ProfileCompletionRequiredAttribute>();
});

builder.Services.AddRazorPages().AddMvcOptions(
    options =>
    {
        options.Filters
            .Add<ProfileCompletionRequiredAttribute>();
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<FollowService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    SeedData.Initialize(services);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseStatusCodePagesWithReExecute("/Error/{0}");
    app.UseExceptionHandler("/Error");
    // app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/Error/{0}");
// app.UseExceptionHandler("/Error");

app.UseHttpsRedirection();
app.UseStaticFiles();


app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    "createPost",
    "Post/Create/{postDestination}",
    new { controller = "Post", action = "Create" });

app.MapControllerRoute(
    "deletePost",
    "Post/Delete/{id}",
    new { controller = "Post", action = "Delete" });

app.MapControllerRoute(
    "showPost",
    "Post/Show/{id}",
    new { controller = "Post", action = "Show" });

app.MapControllerRoute(
    "showComments",
    "Comment/Index/{id}",
    new { controller = "Post", action = "Show" });

app.MapControllerRoute(
    "createGroup",
    "UserGroup/Create",
    new
    {
        controller = "UserGroup",
        action = "Create"
    });

app.MapControllerRoute(
    "createGroup",
    "UserGroup/Show/{groupId}",
    new { controller = "UserGroup", action = "Show" });

app.MapControllerRoute(
    "Profile",
    "Profile/Show/{username?}",
    new { controller = "Profile", action = "Show" });

app.MapControllerRoute(
    "Profile",
    "Profile/FollowersAndFollowing/{username?}",
    new { controller = "Profile", action = "FollowersAndFollowing" });

// app.MapControllerRoute(
//     "ProfileEdit",
//     "{Profile}/{Edit}/",
//     new { controller = "Account", action = "Manage" });

app.MapControllerRoute(
    "default",
    "{controller=Feed}/{action=Index}");
app.MapRazorPages();

app.Run();