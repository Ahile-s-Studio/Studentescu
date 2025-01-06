using Bogus;
using Studentescu.Data;

namespace Studentescu.Models;

public class PostsGenerator
{
    private readonly ApplicationDbContext _dbContext;
    private readonly Faker<Post> _postsFaker;

    public PostsGenerator(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        var userIds = dbContext.Users.Where(u => !u.IsAdmin && u.IsProfileCompleted)
            .Select(u => u.Id).ToList();

        _postsFaker = new Faker<Post>()
            .RuleFor(p => p.Title, f => f.Lorem.Sentence(5)) // Random 5 words sentence
            .RuleFor(p => p.Content, f => f.Lorem.Paragraph()) // Random 3 paragraphs
            .RuleFor(p => p.ContentType,
                f => f.PickRandom("Text", "Image", "Video")) // Random content type
            .RuleFor(p => p.UserId, f => f.PickRandom(userIds)) // Random username for UserId
            .RuleFor(p => p.CreatedAt, f => f.Date.Recent(30)) // Random date in the last 30 days
            // .RuleFor(p => p.UserGroup, f => f.Random.Bool() ? new UserGroup { Id = f.Random.Int(), Name = f.Company.CatchPhrase() } : null) // Random UserGroup (optional)
            .UseSeed(12345);
    }

    public List<Post> GeneratePosts(int count = 10)
    {
        var posts = _postsFaker.Generate(count);
        if (posts == null)
        {
            return [];
        }


        return posts;
    }
}