using Bogus;
using Microsoft.EntityFrameworkCore;
using Studentescu.Data;
using Studentescu.Models;

public class CommentsGenerator
{
    private readonly Faker<Comment> _commentFaker;
    private readonly ApplicationDbContext _dbContext;
    private Faker<Comment> _commentFakerGroupPosts;

    public CommentsGenerator(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;

        var postIds = _dbContext.Posts
            .Select(p => new { p.Id, p.UserId }).ToList();
        var userIds = dbContext.Users.Where(u => !u.IsAdmin && u.IsProfileCompleted)
            .Select(u => u.Id).ToList();

        var genericComments = new List<string>
        {
            "Great post! I totally agree with this.",
            "This is so interesting! Thanks for sharing.",
            "I had no idea about this, but it's really cool.",
            "Can’t wait to try this out!",
            "Wow, this is amazing! Keep it up.",
            "This really made me think. Good stuff!",
            "I love this! Definitely going to share it.",
            "Such a good read, thanks for posting!",
            "I think you nailed it with this one. Well done!",
            "This is really helpful, appreciate the info.",
            "Really well said, couldn't agree more!",
            "This is so true! I love this perspective.",
            "I think you made some great points here.",
            "Interesting! I hadn’t thought about it this way before.",
            "Such a thought-provoking post, thanks for sharing.",
            "I’ve been thinking about this a lot lately too.",
            "This is exactly what I needed to read today.",
            "You’ve given me a lot to think about with this post.",
            "I completely agree with what you’re saying here.",
            "This is such an important topic, glad you're talking about it.",
            "I love how you’ve broken this down. Very insightful!",
            "Great take! I think you’re spot on with this.",
            "This really resonated with me, thank you for posting it.",
            "I’ve learned something new today, thanks for sharing.",
            "Such an inspiring message! Keep it up.",
            "I had no idea about this before, but I’m so glad I read it.",
            "Can’t stop thinking about this post. Very impactful.",
            "Such a refreshing point of view!",
            "I definitely needed this reminder today. Thank you!",
            "This is so relatable, I’m sure a lot of people feel the same."
        };

        _commentFaker = new Faker<Comment>()
            .RuleFor(c => c.Content,
                f => f.PickRandom(genericComments)) // Random sentence or generic comment
            .RuleFor(c => c.CommentedAt, f => f.Date.Recent(30)) // Random date in the last 30 days
            .RuleFor(c => c.PostId,
                f => f.PickRandom(postIds.Select(p => p.Id))) // Random PostId from existing posts
            .RuleFor(c => c.UserId, (f, c) => f.PickRandom(
                userIds.Where(u =>
                    u != postIds.Where(p => p.Id == c.PostId).Select(p => p.UserId)
                        .First()))) // Exclude user from same post
            .UseSeed(12345);


        // .RuleFor(c => c.Replies, f => new List<Comment>()) // No replies initially
        // .RuleFor(c => c.ParentId, f => f.Random.Bool() ? f.Random.Int() : (int?)null); // Random Parent ID
    }

    public List<Comment> GenerateComments(int count = 5)
    {
        var comments = _commentFaker.Generate(count);

        comments[0].Content =
            "Haha, love it! Here’s a quick one: What do you call a dinosaur with an extensive vocabulary? A thesaurus! 🦖📚😂";
        comments[0].PostId = 2;

        comments[1].Content =
            "Haha, that's a good one! Here's mine: Why don't oysters donate to charity? Because they are shellfish! 🦪😂";
        comments[1].PostId = 2;

        comments[2].Content =
            "I needed that laugh! Here's a joke: Why did the scarecrow win an award? Because he was outstanding in his field! 🌾😄";
        comments[2].PostId = 2;

        comments[3].Content =
            "Haha, that joke cracked me up! Here's one for you: Why did the bicycle fall over? Because it was two-tired! 🚲🤣";
        comments[3].PostId = 2;

        comments[4].Content =
            "Great way to start the day with a laugh! Here's mine: Why did the tomato turn red? Because it saw the salad dressing! 🍅😆";
        comments[4].PostId = 2;

        comments[5].Content =
            "I’m dying of laughter over here! 😂 How about this one: What do you call fake spaghetti? An impasta! 🍝😜";
        comments[5].PostId = 2;

        comments[6].Content =
            "Haha! That joke was so funny! Here's mine: Why can't your nose be 12 inches long? Because then it would be a foot! 👃🤣";
        comments[6].PostId = 2;

        comments[7].Content =
            "Laughter really is the best medicine! Here’s my contribution: What do you get when you cross a snowman and a vampire? Frostbite! ☃️🧛‍♂️😄";
        comments[7].PostId = 2;

        comments[8].Content =
            "I love a good joke! Here’s mine: Why don’t skeletons ever use cell phones? They don’t have the nerve! ☠️📱😂";
        comments[8].PostId = 2;

        comments[9].Content =
            "That was hilarious! Okay, here’s one: Why do cows have hooves instead of feet? Because they lactose! 🐄😆";
        comments[9].PostId = 2;

        return comments ?? [];
    }

    public async Task<List<Comment>> GenerateCommentsForGroupPosts(int count = 5)
    {
        List<Comment> comments = [];

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

            _commentFakerGroupPosts = new Faker<Comment>()
                .RuleFor(c => c.Content,
                    f => f.PickRandom(f.Lorem.Sentence(10), GenerateRandomComment()))
                .RuleFor(c => c.CommentedAt, f => f.Date.Recent(30))
                .RuleFor(c => c.PostId, f => f.PickRandom(postIds))
                .RuleFor(c => c.UserId, (f, c) => f.PickRandom(userIds))
                .UseSeed(12345);
            comments.AddRange(_commentFakerGroupPosts.Generate(count));
        }

        return comments ?? [];
    }

    private static string GenerateRandomComment()
    {
        string[] comments =
        {
            "This is a great post, thanks for sharing!",
            "I really enjoyed this, very informative!",
            "This is quite insightful, keep it up!",
            "Thanks for the post, I learned something new today.",
            "Interesting content, looking forward to more!",
            "I can relate to this, great job!",
            "This is a thought-provoking piece, well done!",
            "I appreciate the effort you put into this, nice work!",
            "Good read, I’ll definitely think about this more.",
            "I love the perspective in this, very refreshing!"
        };

        var rand = new Random();
        var index = rand.Next(comments.Length); // Get a random index
        return comments[index];
    }
}