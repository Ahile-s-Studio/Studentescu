using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Studentescu.Data;

namespace Studentescu.Models;

public class SeedData
{
    public static void Initialize(
        IServiceProvider serviceProvider)
    {
        using (var context = new ApplicationDbContext(
                   serviceProvider.GetRequiredService
                   <DbContextOptions<
                       ApplicationDbContext>>()))
        {
            if (!context.Roles.Any())
            {
                context.Roles.AddRange(
                    new IdentityRole
                    {
                        Id =
                            "2c5e174e-3b0e-446f-86af-483d56fd7210",
                        Name = "Admin",
                        NormalizedName =
                            "Admin".ToUpper()
                    },
                    // new IdentityRole { Id = "2c5e174e-3b0e-446f-86af-483d56fd7211", Name = "Editor", NormalizedName = "Editor".ToUpper() },
                    new IdentityRole
                    {
                        Id =
                            "2c5e174e-3b0e-446f-86af-483d56fd7212",
                        Name = "User",
                        NormalizedName =
                            "User".ToUpper()
                    }
                );
            }


            // var hasher = new PasswordHasher<ApplicationUser>();

            // context.Users.AddRange(
            //     new ApplicationUser
            //     {
            //         Id = "8e445865-a24d-4543-a6c6-9443d048cdb0", // primary key
            //         UserName = "admin@test.com",
            //         EmailConfirmed = true,
            //         NormalizedEmail = "ADMIN@TEST.COM",
            //         Email = "admin@test.com",
            //         NormalizedUserName = "ADMIN@TEST.COM",
            //         PasswordHash = hasher.HashPassword(null, "Admin1!")
            //     },
            //
            //     new ApplicationUser
            //     {
            //         Id = "8e445865-a24d-4543-a6c6-9443d048cdb1", // primary key
            //         UserName = "editor@test.com",
            //         EmailConfirmed = true,
            //         NormalizedEmail = "EDITOR@TEST.COM",
            //         Email = "editor@test.com",
            //         NormalizedUserName = "EDITOR@TEST.COM",
            //         PasswordHash = hasher.HashPassword(null, "Editor1!")
            //     },
            //
            //     new ApplicationUser
            //     {
            //         Id = "8e445865-a24d-4543-a6c6-9443d048cdb2", // primary key
            //         UserName = "user@test.com",
            //         EmailConfirmed = true,
            //         NormalizedEmail = "USER@TEST.COM",
            //         Email = "user@test.com",
            //         NormalizedUserName = "USER@TEST.COM",
            //         PasswordHash = hasher.HashPassword(null, "User1!")
            //     }
            // );
            //
            //
            // context.UserRoles.AddRange(
            //     new IdentityUserRole<string>
            //     {
            //         RoleId = "2c5e174e-3b0e-446f-86af-483d56fd7210",
            //         UserId = "8e445865-a24d-4543-a6c6-9443d048cdb0"
            //     },
            //     new IdentityUserRole<string>
            //     {
            //         RoleId = "2c5e174e-3b0e-446f-86af-483d56fd7211",
            //         UserId = "8e445865-a24d-4543-a6c6-9443d048cdb1"
            //     },
            //     new IdentityUserRole<string>
            //     {
            //         RoleId = "2c5e174e-3b0e-446f-86af-483d56fd7212",
            //         UserId = "8e445865-a24d-4543-a6c6-9443d048cdb2"
            //     }
            // );

            context.SaveChanges();
        }

        using (var userManager =
               serviceProvider.GetRequiredService
                   <UserManager<ApplicationUser>>())
        {
            // Creare admin inițial   
            var user = userManager
                .FindByEmailAsync("admin@example.com")
                .Result;
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = "admin@example.com",
                    Email = "admin@example.com",
                    EmailConfirmed = true
                };

                var result = userManager
                    .CreateAsync(user, "Admin123!")
                    .Result;

                if (result.Succeeded)
                {
                    userManager
                        .AddToRoleAsync(user, "Admin")
                        .Wait();
                }
            }


            // Create 5 Regular Users
            for (var i = 1; i <= 5; i++)
            {
                var username = $"user{i}";
                var firstName = $"User{i}FirstName";
                var lastName = $"User{i}LastName";
                var email = $"user{i}@example.com";
                var normalUser = userManager
                    .FindByEmailAsync(email).Result;

                if (normalUser != null)
                {
                    continue;
                }

                normalUser = new ApplicationUser
                {
                    FirstName = firstName,
                    LastName = lastName,
                    UserName = username,
                    Email = email,
                    EmailConfirmed = true,
                    Public = false
                };

                var userResult = userManager
                    .CreateAsync(normalUser,
                        $"User{i}123!").Result;


                if (userResult.Succeeded)
                {
                    userManager
                        .AddToRoleAsync(normalUser, "User")
                        .Wait();
                    continue;
                }

                Console.WriteLine(
                    $"Failed to create user {email}: {string.Join(", ", userResult.Errors.Select(e => e.Description))}");
            }
        }
    }
}