using Bogus;
using Studentescu.Data;

namespace Studentescu.Models;

public class FollowsGenerator
{
    private readonly ApplicationDbContext _dbContext;
    private readonly Faker _faker;

    public FollowsGenerator(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        _faker = new Faker();
    }

    // Generate follow relationships
    public List<Follow> GenerateFollows(int count = 10)
    {
        var userIds = _dbContext.Users
            .Where(u => !u.IsAdmin && u.IsProfileCompleted)
            .Select(u => u.Id)
            .ToList();
        var publicUserIds = _dbContext.Users
            .Where(u => !u.IsAdmin && u.IsProfileCompleted && u.Public)
            .Select(u => u.Id)
            .ToList();

        if (userIds.Count < 2)
        {
            throw new InvalidOperationException("Not enough users to generate follows.");
        }

        var follows = new List<Follow>();
        // var existingFollows = _dbContext.Follows
        //     .Select(f => new { f.FollowerId, f.FolloweeId })
        //     .ToHashSet();

        var existingFollows = _dbContext.Follows
            .Select(f => new { f.FollowerId, f.FolloweeId })
            .Union(
                _dbContext.FollowRequests
                    .Select(fr => new { FollowerId = fr.RequesterId, FolloweeId = fr.TargetId })
            )
            .ToHashSet();

        // var existingFollows = _dbContext.FollowRequests
        //     .Select(fr => new { fr.RequesterId, fr.TargetId })
        //     .Union(
        //         _dbContext.Follows
        //             .Select(f => new { RequesterId = f.FollowerId, TargetId = f.FolloweeId })
        //     )
        //     .ToHashSet();

        var c = 0;
        while (follows.Count < count)
        {
            if (c++ > 10000)
            {
                break;
            }

            var followerId = _faker.PickRandom(userIds);
            // var followeeId = _faker.PickRandom(userIds);
            var followeeId = _faker.PickRandom(publicUserIds);

            if (followerId == followeeId ||
                existingFollows.Contains(new { FollowerId = followerId, FolloweeId = followeeId }))
            {
                continue;
            }

            var follow = new Follow
            {
                FollowerId = followerId,
                FolloweeId = followeeId,
                FollowedAt = DateTime.Now
            };

            follows.Add(follow);
            existingFollows.Add(new { FollowerId = followerId, FolloweeId = followeeId });
        }

        return follows;
    }

    // Generate follow requests
    public List<FollowRequest> GenerateFollowRequests(int count = 10)
    {
        var userIds = _dbContext.Users
            .Where(u => !u.IsAdmin && u.IsProfileCompleted)
            .Select(u => u.Id)
            .ToList();

        var privateUserIds = _dbContext.Users
            .Where(u => !u.IsAdmin && u.IsProfileCompleted && !u.Public)
            .Select(u => u.Id)
            .ToList();

        if (userIds.Count < 2)
        {
            throw new InvalidOperationException("Not enough users to generate follow requests.");
        }

        var followRequests = new List<FollowRequest>();
        // var existingRequests = _dbContext.FollowRequests
        //     .Select(fr => new { fr.RequesterId, fr.TargetId })
        //     .ToHashSet();

        var existingRequests = _dbContext.FollowRequests
            .Select(fr => new { fr.RequesterId, fr.TargetId })
            .Union(
                _dbContext.Follows
                    .Select(f => new { RequesterId = f.FollowerId, TargetId = f.FolloweeId })
            )
            .ToHashSet();

        var c = 0;
        while (followRequests.Count < count)
        {
            if (c++ > 10000)
            {
                break;
            }

            var requesterId = _faker.PickRandom(userIds);
            // var targetId = _faker.PickRandom(userIds);
            var targetId = _faker.PickRandom(privateUserIds);

            if (requesterId == targetId ||
                existingRequests.Contains(new { RequesterId = requesterId, TargetId = targetId }))
            {
                continue;
            }

            var followRequest = new FollowRequest
            {
                RequesterId = requesterId,
                TargetId = targetId,
                Status = _faker.PickRandom<FollowRequestStatus>(), // Random status
                CreatedAt = DateTime.Now,
                UpdatedAt = _faker.Date.Recent(5) // Random recent update
            };

            followRequests.Add(followRequest);
            existingRequests.Add(new { RequesterId = requesterId, TargetId = targetId });
        }

        return followRequests;
    }
}