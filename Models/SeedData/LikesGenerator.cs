using Bogus;
using Microsoft.EntityFrameworkCore;
using Studentescu.Data;

namespace Studentescu.Models;

public class LikesGenerator
{
    private readonly ApplicationDbContext _dbContext;
    private readonly Faker<Like> _likeFaker;
    private Faker<Like> _likeFakerGroupPosts;

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

    public async Task<List<Like>> GenerateLikesForGroupPosts(int count = 5)
    {
        List<Like> comments = [];
        foreach (var groupId in await _dbContext.UserGroups.Select(u => u.Id).ToListAsync())
        {
            var userIds = await _dbContext.MemberInGroups.Where(m => m.UserGroupId == groupId)
                .Select(m => m.UserId).ToListAsync();
            var postIds = await _dbContext.Posts
                .Where(p => p.GroupId != null && p.GroupId == groupId)
                .Select(p => p.Id)
                .ToListAsync();

            if (postIds.Count == 0 || userIds.Count == 0)
            {
                continue;
            }

            _likeFakerGroupPosts = new Faker<Like>()
                .RuleFor(l => l.UserId, f => f.PickRandom(userIds)) // Random User ID
                .RuleFor(l => l.PostId, f => f.PickRandom(postIds)) // Random Post ID
                .RuleFor(l => l.LikedAt,
                    f => f.Date.Recent(30)) // Random LikedAt within last 30 days
                .UseSeed(12345);
            for (var i = 0; i < count; i++)
            {
                var like = _likeFakerGroupPosts.Generate();
                var c = 100;
                // Check if the like already exists in the database (same UserId and PostId)
                while (comments.Any(l => l.UserId == like.UserId && l.PostId == like.PostId) &&
                       c-- > 0)
                {
                    // If duplicate, regenerate the like
                    like = _likeFaker.Generate();
                }

                comments.Add(like);
            }
        }

        return comments ?? [];
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