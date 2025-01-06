using Bogus;
using Studentescu.Data;
using Studentescu.Models;

public class CommentsGenerator
{
    private readonly Faker<Comment> _commentFaker;
    private readonly ApplicationDbContext _dbContext;

    public CommentsGenerator(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        var postIds = _dbContext.Posts
            .Select(u => u.Id).ToList();
        var userIds = dbContext.Users.Where(u => !u.IsAdmin && u.IsProfileCompleted)
            .Select(u => u.Id).ToList();

        _commentFaker = new Faker<Comment>()
            .RuleFor(c => c.Content, f => f.Lorem.Sentence(10)) // Random content (10 words)
            .RuleFor(c => c.CommentedAt, f => f.Date.Recent(30)) // Random date in the last 30 days
            .RuleFor(c => c.PostId, f => f.PickRandom(postIds)) // Random date in the last 30 days
            .RuleFor(c => c.UserId, f => f.PickRandom(userIds)); // Random date in the last 30 days
        // .RuleFor(c => c.Replies, f => new List<Comment>()) // No replies initially
        // .RuleFor(c => c.ParentId, f => f.Random.Bool() ? f.Random.Int() : (int?)null); // Random Parent ID
    }

    public List<Comment> GenerateComments(int count = 5)
    {
        var comments = _commentFaker.Generate(count);

        return comments ?? [];
    }
}