using Bogus;
using Studentescu.Data;

namespace Studentescu.Models;

public class UserGroupsGenerator
{
    private readonly ApplicationDbContext _dbContext;
    private readonly Faker _faker = new();
    private readonly Faker<UserGroup> _userGroupFaker;

    public UserGroupsGenerator(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        var userIds = dbContext.Users.Where(u => !u.IsAdmin && u.IsProfileCompleted)
            .Select(u => u.Id).ToList();

        _userGroupFaker = new Faker<UserGroup>()
            .RuleFor(g => g.Name, f => f.Company.CompanyName()) // Random group name
            .RuleFor(g => g.Description, f => f.Lorem.Sentence(10)) // Random description
            .RuleFor(g => g.Category, f => f.PickRandom<CategoryType>()) // Random category
            .RuleFor(g => g.GroupImageUrl, f => f.Image.PicsumUrl()) // Random group image URL
            // .RuleFor(g => g.Active, f => f.Random.Bool()) // Random active status
            .RuleFor(g => g.Active, f => true) // Random active status
            .RuleFor(g => g.CreatedAt,
                f => f.Date.Recent(90)) // Random created date (within the last 90 days)
            .RuleFor(g => g.Members,
                f => new List<MemberInGroup>()) // Empty members list (to be populated later)
            .RuleFor(g => g.GroupPosts,
                f => new List<Post>()) // Empty posts list (to be populated later)
            .UseSeed(12345); // Optional seed for reproducibility
    }

    public List<UserGroup> GenerateUserGroups(int count = 8)
    {
        var userGroups = _userGroupFaker.Generate(count);
        var userIds = _dbContext.Users.Where(u => !u.IsAdmin && u.IsProfileCompleted)
            .Select(u => u.Id).ToList();

        // For example: Assign some random members to each group
        foreach (var group in userGroups)
        {
            var randomMemberCount = new Random().Next(5, 10); // Random number of members
            for (var i = 0; i < randomMemberCount; i++)
            {
                // var randomUserId = _dbContext.Users.Where(u => !u.IsAdmin && u.IsProfileCompleted)
                //     .Select(u => u.Id).OrderBy(x => Guid.NewGuid()).First(); // Get random user id
                var randomUserId = userIds[new Random().Next(userIds.Count)];

                if (group.Members.All(m => m.UserId != randomUserId))
                {
                    group.Members.Add(new MemberInGroup
                    {
                        UserId = randomUserId,
                        UserGroupId = group.Id,
                        Role = i == 0
                            ? GroupRole.Admin
                            : _faker.PickRandom(GroupRole
                                .Moderator, GroupRole.Member)
                    });
                }
            }
        }

        return userGroups;
    }
}