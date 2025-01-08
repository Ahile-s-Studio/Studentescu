using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Studentescu.Data;

namespace Studentescu.Models;

public class SeedData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using var context = new ApplicationDbContext(
            serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());
        if (context.Users.Any())
        {
            return;
        }

        SeedRoles(serviceProvider);

        SeedUsers(serviceProvider);

        SeedUserGroups(serviceProvider);

        SeedPosts(serviceProvider);

        SeedComments(serviceProvider);

        SeedLikes(serviceProvider);
    }


    private static void SeedRoles(
        IServiceProvider serviceProvider)
    {
        using var context = new ApplicationDbContext(
            serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());
        // Ensure the database is created
        context.Database.EnsureCreated();

        // Seed roles if they don't exist
        if (context.Roles.Any())
        {
            return;
        }

        context.Roles.AddRange(
            new IdentityRole
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Admin",
                NormalizedName = "ADMIN"
            },
            new IdentityRole
            {
                Id = Guid.NewGuid().ToString(),
                Name = "User",
                NormalizedName = "USER"
            }
        );
        context.SaveChanges();
    }

    private static void SeedUsers(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        // Seed Admin User
        SeedAdminUser(userManager, roleManager);

        // Seed Regular Users
        SeedRegularUsers(userManager, roleManager);
    }

    private static void SeedAdminUser(UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        const string adminEmail = "admin@example.com";
        const string adminPassword = "Admin123!";


        if (userManager.FindByEmailAsync(adminEmail).Result != null)
        {
            return;
        }

        var admin = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true,
            IsAdmin = true
        };

        var result = userManager.CreateAsync(admin, adminPassword).Result;

        if (result.Succeeded)
        {
            if (!roleManager.RoleExistsAsync("Admin").Result)
            {
                roleManager.CreateAsync(new IdentityRole("Admin")).Wait();
            }

            userManager.AddToRoleAsync(admin, "Admin").Wait();
        }
        else
        {
            Console.WriteLine(
                $"Failed to create admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }
    }

    private static void SeedRegularUsers(UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        var usersGenerator = new UsersGenerator();
        var users = usersGenerator.GenerateUsers();

        foreach (var user in users)
        {
            var result = userManager.CreateAsync(user, "Bogus123!").Result;

            if (result.Succeeded)
            {
                if (!roleManager.RoleExistsAsync("User").Result)
                {
                    roleManager.CreateAsync(new IdentityRole("User")).Wait();
                }

                userManager.AddToRoleAsync(user, "User").Wait();
            }
            else
            {
                Console.WriteLine(
                    $"Failed to create user {user.Email}: {string.Join(", ", result.Errors.Select(e => e
                        .Description))}");
            }
        }

        // generate mock users
        for (var i = 1; i <= 2; i++)
        {
            var email = $"user{i}@example.com";
            var userName = $"user{i}";

            if (userManager.FindByEmailAsync(email).Result != null)
            {
                continue;
            }

            var user = new ApplicationUser
            {
                UserName = userName,
                Email = email,
                EmailConfirmed = true,
                IsAdmin = false
            };

            var result = userManager.CreateAsync(user, $"User{i}123!").Result;

            if (result.Succeeded)
            {
                if (!roleManager.RoleExistsAsync("User").Result)
                {
                    roleManager.CreateAsync(new IdentityRole("User")).Wait();
                }

                userManager.AddToRoleAsync(user, "User").Wait();
            }
            else
            {
                Console.WriteLine(
                    $"Failed to create user {email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }
    }

    private static void SeedPosts(IServiceProvider serviceProvider)
    {
        using var context = new ApplicationDbContext(
            serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());
        context.Database.EnsureCreated();
        var postsGenerator = new PostsGenerator(context,
            serviceProvider.GetRequiredService<IConfiguration>());
        var posts = postsGenerator.GeneratePosts(25);
        context.Posts.AddRange(posts);
        context.SaveChanges();
    }

    private static void SeedComments(IServiceProvider serviceProvider)
    {
        using var context = new ApplicationDbContext(
            serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());
        context.Database.EnsureCreated();
        var commentsGenerator = new CommentsGenerator(context);
        var comments = commentsGenerator.GenerateComments(90);
        context.Comments.AddRange(comments);
        context.SaveChanges();
    }

    private static void SeedLikes(IServiceProvider serviceProvider)
    {
        using var context = new ApplicationDbContext(
            serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());
        context.Database.EnsureCreated();
        var likesGenerator = new LikesGenerator(context);
        var likes = likesGenerator.GenerateLikes(90);
        context.Likes.AddRange(likes);
        context.SaveChanges();
    }

    private static void SeedUserGroups(IServiceProvider serviceProvider)
    {
        using var context = new ApplicationDbContext(
            serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());
        context.Database.EnsureCreated();
        var groupsGenerator = new UserGroupsGenerator(context);
        var likes = groupsGenerator.GenerateUserGroups();
        context.UserGroups.AddRange(likes);
        context.SaveChanges();
    }
}