using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Studentescu.Data;
using Studentescu.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection") ??
    throw new InvalidOperationException(
        "Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString,
        new MySqlServerVersion(new Version(8, 0, 33))));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
        options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();


app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "createPost",
    pattern: "Post/Create/{postDestination}",
    defaults: new { controller = "Post", action = "Create" });

app.MapControllerRoute(
    name: "deletePost",
    pattern: "Post/Delete/{id}",
    defaults: new { controller = "Post", action = "Delete" });

app.MapControllerRoute(
    name: "showPost",
    pattern: "Post/Show/{id}",
    defaults: new { controller = "Post", action = "Show" });

app.MapControllerRoute(
    name: "createGroup",
    pattern: "UserGroup/Create",
    defaults: new { controller = "UserGroup", action = "Create" });

app.MapControllerRoute(
    name: "createGroup",
    pattern: "UserGroup/Show/{groupId}",
    defaults: new { controller = "UserGroup", action = "Show" });

app.MapControllerRoute(
    "Profile",
    "Profile/Show/{username?}",
    new { controller = "Profile", action = "Show" });

// app.MapControllerRoute(
//     "ProfileEdit",
//     "{Profile}/{Edit}/",
//     new { controller = "Account", action = "Manage" });

app.MapControllerRoute(
    "default",
    "{controller=Feed}/{action=Index}");
app.MapRazorPages();

app.Run();