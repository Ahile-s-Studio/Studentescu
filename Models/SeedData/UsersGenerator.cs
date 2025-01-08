using Bogus;

namespace Studentescu.Models;

public class UsersGenerator
{
    private readonly Faker<ApplicationUser> _userFaker;

    public UsersGenerator()
    {
        _userFaker = new Faker<ApplicationUser>()
            .RuleFor(u => u.FirstName, f => f.Name.FirstName())
            .RuleFor(u => u.LastName, f => f.Name.LastName())
            .RuleFor(u => u.UserName, (f, u) => f.Internet.UserName(u.FirstName, u.LastName))
            .RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.UserName))
            // .RuleFor(u => u.ProfilePictureUrl, f => f.Internet.Avatar())
            .RuleFor(u => u.ProfilePictureUrl, f => f.Image.PicsumUrl())
            .RuleFor(u => u.Biography, f => f.Lorem.Sentence())
            .RuleFor(u => u.Public, f => f.Random.Bool())
            // .RuleFor(u => u.Public, f => true)
            .RuleFor(u => u.IsProfileCompleted, f => true)
            .RuleFor(u => u.EmailConfirmed, f => true)
            .UseSeed(12345);
    }

    public List<ApplicationUser> GenerateUsers(int count = 50)
    {
        var users = _userFaker.Generate(count);
        if (users == null)
        {
            return [];
        }

        users[0].Public = true;
        users[1].Public = true;

        return users;
    }
}