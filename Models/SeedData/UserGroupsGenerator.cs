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

        var genericGroupNames = new List<string>
        {
            "The Innovators Collective",
            "The Knowledge Exchange",
            "Creative Minds Hub",
            "The Learning Network",
            "Future Leaders Forum",
            "Tech Enthusiasts United",
            "The Visionaries Circle",
            "Global Collaboration Group",
            "The Idea Factory",
            "Inspiration Nation",
            "The Growth Academy",
            "Explorers of Innovation",
            "The Thought Leaders Group",
            "Community of Creators",
            "The Project Pioneers",
            "Dreamers and Doers",
            "The Connection Zone",
            "Growth Seekers",
            "The Collaboration Crew",
            "The Insight Network"
        };

        var genericGroupDescriptions = new List<string>
        {
            "A group dedicated to sharing knowledge and ideas.",
            "A community of like-minded individuals with a passion for growth.",
            "A place to collaborate and discuss various topics of interest.",
            "Join us for engaging discussions and new perspectives.",
            "This is a space for learning and personal development.",
            "A group where you can connect with others and share experiences.",
            "A creative community focused on innovation and exploration.",
            "A hub for passionate individuals to share their ideas and projects.",
            "A place to inspire and be inspired by others.",
            "This group is about connecting people with similar interests."
        };

        _userGroupFaker = new Faker<UserGroup>()
            .RuleFor(g => g.Name,
                f => f.PickRandom(genericGroupNames)) // Random group name from generic list
            .RuleFor(g => g.Description,
                f => f.PickRandom(genericGroupDescriptions)) // Random description from generic list
            .RuleFor(g => g.Category, f => f.PickRandom<CategoryType>()) // Random category
            .RuleFor(g => g.GroupImageUrl, f => f.Image.PicsumUrl()) // Random group image URL
            .RuleFor(g => g.Active, f => true) // Group is always active
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
        const string adminUsername = "ANNABELLE_BERNIER79";
        var adminUserId = _dbContext.Users
            .Where(u => u.NormalizedUserName == adminUsername)
            .Select(u => u.Id)
            .FirstOrDefault(); // Retrieve the admin's user ID

        foreach (var group in userGroups)
        {
            var randomMemberCount = new Random().Next(5, 10); // Random number of members

            // Ensure the first group gets a custom admin
            if (group == userGroups.First() && adminUserId != null)
            {
                group.Members.Add(new MemberInGroup
                {
                    UserId = adminUserId,
                    UserGroupId = group.Id,
                    Role = GroupRole.Admin
                });
                randomMemberCount--; // Reduce member count as admin is already added
            }

            for (var i = 0; i < randomMemberCount; i++)
            {
                var randomUserId = userIds[new Random().Next(userIds.Count)];

                // Avoid duplicates in group members
                if (group.Members.All(m => m.UserId != randomUserId))
                {
                    group.Members.Add(new MemberInGroup
                    {
                        UserId = randomUserId,
                        UserGroupId = group.Id,
                        Role = i == 0 && group.Members.Count == 0
                            ? GroupRole.Admin
                            : _faker.PickRandom(GroupRole.Moderator, GroupRole.Member)
                    });
                }
            }
        }


        return userGroups;
    }

    public List<JoinRequest> GenerateJoinRequests()
    {
        var groups = _dbContext.UserGroups;
        var joinRequests = new List<JoinRequest>();
        var userIds = _dbContext.Users.Where(u => !u.IsAdmin && u.IsProfileCompleted)
            .Select(u => u.Id).ToList();
        foreach (var admin in groups.Select(group => _dbContext.MemberInGroups.FirstOrDefault(u =>
                     u.Role == GroupRole.Admin
                     && u.UserGroupId == group.Id)))
        {
            if (admin == null)
            {
                continue;
            }

            for (var i = 0; i < 5; i++)
            {
                var randomUserId = userIds[new Random().Next(userIds.Count)];
                joinRequests.Add(new JoinRequest
                {
                    RequesterId = randomUserId,
                    TargetId = admin.UserId,
                    GroupId = admin.UserGroupId,
                    Status = JoinRequestStatus.Pending
                });
            }
        }

        return joinRequests;
    }
}