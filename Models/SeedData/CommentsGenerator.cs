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
            .Select(p => new { p.Id, p.UserId }).ToList();
        var userIds = dbContext.Users.Where(u => !u.IsAdmin && u.IsProfileCompleted)
            .Select(u => u.Id).ToList();

        _commentFaker = new Faker<Comment>()
            .RuleFor(c => c.Content,
                f => f.PickRandom(f.Lorem.Sentence(10), GenerateRandomComment()))
            .RuleFor(c => c.CommentedAt, f => f.Date.Recent(30))
            .RuleFor(c => c.PostId, f => f.PickRandom(postIds.Select(p => p.Id)))
            .RuleFor(c => c.UserId, (f, c) => f.PickRandom(
                userIds.Where(u => u != postIds.Where
                    (p => p.Id == c.PostId).Select(p => p.UserId).First()))
            )
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