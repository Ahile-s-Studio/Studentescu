using Bogus;
using Studentescu.Data;

namespace Studentescu.Models;

public class LikesGenerator
{
    private readonly ApplicationDbContext _dbContext;
    private readonly Faker<Like> _likeFaker;

    public LikesGenerator(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;

        // Get list of post IDs and user IDs from the database
        var postIds = _dbContext.Posts.Select(p => p.Id).ToList();
        var userIds = _dbContext.Users.Where(u => !u.IsAdmin && u.IsProfileCompleted)
            .Select(u => u.Id).ToList();

        // Initialize Faker for generating Likes
        _likeFaker = new Faker<Like>()
            .RuleFor(l => l.UserId, f => f.PickRandom(userIds)) // Random User ID
            .RuleFor(l => l.PostId, f => f.PickRandom(postIds)) // Random Post ID
            .RuleFor(l => l.LikedAt, f => f.Date.Recent(30)) // Random LikedAt within last 30 days
            .UseSeed(12345);
    }

    // Generate a specified number of likes
    public List<Like> GenerateLikes(int count = 5)
    {
        var generatedLikes = new List<Like>();
        var existingLikes = _dbContext.Likes.ToList(); // Fetch all existing likes from the database

        for (var i = 0; i < count; i++)
        {
            // Generate a like
            var like = _likeFaker.Generate();

            var c = 100;
            // Check if the like already exists in the database (same UserId and PostId)
            while (existingLikes.Any(l => l.UserId == like.UserId && l.PostId == like.PostId) &&
                   c-- > 0)
            {
                // If duplicate, regenerate the like
                like = _likeFaker.Generate();
            }

            generatedLikes.Add(like);
        }

        return generatedLikes;
    }
}