using Microsoft.EntityFrameworkCore;
using Studentescu.Data;
using Studentescu.Models;

namespace Studentescu.Services;

public class FollowService
{
    private readonly ApplicationDbContext _dbContext;

    public FollowService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Follow?> IsFollowing(string currentUserId, string targetUserId)
    {
        return await _dbContext.Follows.FirstOrDefaultAsync(f =>
            f.FollowerId == currentUserId && f.FolloweeId == targetUserId);
    }

    public async Task<FollowRequest?> IsRequestSent(string currentUserId, string targetUserId)
    {
        return await _dbContext.FollowRequests.FirstOrDefaultAsync(f =>
            f.RequesterId == currentUserId && f.TargetId == targetUserId);
    }

    public async Task<bool> AcceptRequest(string currentUserId, int requestId)
    {
        var followRequest = await GetRequest(currentUserId, requestId);
        if (followRequest == null)
        {
            return false;
        }

        followRequest.Status = FollowRequestStatus.Accepted;
        followRequest.UpdatedAt = DateTime.Now;
        _dbContext.FollowRequests.Update(followRequest);

        var follow = new Follow
        {
            FollowerId = followRequest.RequesterId,
            FolloweeId = followRequest.TargetId
        };
        _dbContext.Follows.Add(follow);
        return await _dbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> RejectRequest(string currentUserId, int requestId)
    {
        var followRequest = await GetRequest(currentUserId, requestId);
        if (followRequest == null)
        {
            return false;
        }

        followRequest.Status = FollowRequestStatus.Rejected;
        followRequest.UpdatedAt = DateTime.Now;
        _dbContext.FollowRequests.Update(followRequest);

        return await _dbContext.SaveChangesAsync() > 0;
    }

    public async Task<FollowRequest?> GetRequest(string receiverId, int requestId)
    {
        var followRequest = await _dbContext.FollowRequests.FirstOrDefaultAsync(fr =>
            fr.TargetId == receiverId && fr.Id == requestId);
        return followRequest;
    }

    public async Task<List<FollowRequest>> GetPendingRequestsSentBy(string currentUserId)
    {
        var pendingRequestsSent = await _dbContext.FollowRequests
            .Where(fr =>
                fr.RequesterId == currentUserId && fr.Status == FollowRequestStatus.Pending)
            // .Include(fr => fr.Requester)
            .ToListAsync();
        return pendingRequestsSent;
    }
}