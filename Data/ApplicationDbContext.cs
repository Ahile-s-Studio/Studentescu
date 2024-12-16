using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Studentescu.Models;

namespace Studentescu.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<Post> Posts { get; set; } 
    
    public DbSet<Comment> Comments { get; set; }
    
    public DbSet<Like> Likes { get; set; }
    
    public DbSet<UserGroup> UserGroups { get; set; }
    
    public DbSet<MemberInGroup> MemberInGroups { get; set; }
    
    public DbSet<Follow> Follows { get; set; }
    
    public DbSet<MemberInGroup> GroupMemberships { get; set; }
    
    public DbSet<FollowRequest> FollowRequests { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Follow Relationship
        builder.Entity<Follow>()
            .HasOne(f => f.Follower)
            .WithMany(u => u.Following)
            .HasForeignKey(f => f.FollowerId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<Follow>()
            .HasOne(f => f.Followee)
            .WithMany(u => u.Followers)
            .HasForeignKey(f => f.FolloweeId)
            .OnDelete(DeleteBehavior.NoAction);

        // Like Relationship 
        builder.Entity<Like>()
            .HasOne(l => l.User)
            .WithMany(u => u.Likes)
            .HasForeignKey(l => l.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Like>()
            .HasOne(l => l.Post)
            .WithMany(p => p.Likes)
            .HasForeignKey(l => l.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        // Comment Relationship 
        builder.Entity<Comment>()
            .HasOne(c => c.Post)
            .WithMany(p => p.Comments)
            .HasForeignKey(c => c.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Comment>()
            .HasOne(c => c.User)
            .WithMany(u => u.Comments)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Comment>()
            .HasOne(c => c.Parent)
            .WithMany(c => c.Replies)
            .HasForeignKey(c => c.ParentId)
            .OnDelete(DeleteBehavior.SetNull);


        // FollowRequest Relationship 
        builder.Entity<FollowRequest>()
            .HasOne(f => f.Requester)
            .WithMany(u => u.RequestsSent)
            .HasForeignKey(f => f.RequesterId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<FollowRequest>()
            .HasOne(f => f.Target)
            .WithMany(u => u.RequestsReceived)
            .HasForeignKey(f => f.TargetId)
            .OnDelete(DeleteBehavior.NoAction);

        // Message Relationship 
        builder.Entity<Message>()
            .HasOne(m => m.Sender)
            .WithMany(u => u.MessagesSent)
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<PrivateMessage>()
            .HasOne(m => m.Receiver)
            .WithMany(u => u.PrivateMessagesReceived)
            .HasForeignKey(m => m.ReceiverId)
            .OnDelete(DeleteBehavior.NoAction);

        /*builder.Entity<GroupMessage>()
            .HasOne(m => m.UserGroup)
            .WithMany(u => u.GroupMessages)
            .HasForeignKey(m => m.UserGroupId)
            .OnDelete(DeleteBehavior.NoAction);*/

        // UserGroup Relationship 
        builder.Entity<MemberInGroup>()
            .HasOne(mg => mg.User)
            .WithMany(u => u.GroupMemberships)
            .HasForeignKey(mg => mg.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<MemberInGroup>()
            .HasOne(mg => mg.UserGroup)
            .WithMany(u => u.Members)
            .HasForeignKey(mg => mg.UserGroupId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}