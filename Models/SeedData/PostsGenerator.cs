using Bogus;
using Studentescu.Data;
using Studentescu.Utils;

namespace Studentescu.Models;

public class PostsGenerator
{
    private readonly ApplicationDbContext _dbContext;
    private readonly Faker _faker = new();
    private readonly Faker<Post> _postsFaker;

    public PostsGenerator(ApplicationDbContext dbContext, IConfiguration configuration)
    {
        _dbContext = dbContext;
        var userIds = dbContext.Users.Where(u => !u.IsAdmin && u.IsProfileCompleted)
            .Select(u => u.Id).ToList();
        var groupIds = dbContext.UserGroups.Select(ug => ug.Id).ToList();

        // const string
        //     youtubeApiKey = "AIzaSyDulzGwP1TxHnEbdCCumL_pwQvGMT881eE"; // Replace with your API Key
        var
            youtubeApiKey = configuration["GOOGLE_API_KEY"];
        var youtubeRandomVideos = new YouTubeRandomVideos();

        // Get 15 random YouTube embed URLs
        var randomVideoUrls =
            // youtubeRandomVideos.GetRandomCatYouTubeEmbedUrlsAsync(youtubeApiKey).Result;
            new List<string>
            {
                "https://www.youtube.com/embed/LUiJNmY-P48",
                "https://www.youtube.com/embed/ePizt97VWLI",
                "https://www.youtube.com/embed/jT11MOENfio",
                "https://www.youtube.com/embed/NsMKvVdEPkw",
                "https://www.youtube.com/embed/yJ0WVA_GUOI",
                "https://www.youtube.com/embed/HT5m7CQ40Fk",
                "https://www.youtube.com/embed/HT5m7CQ40Fk",
                "https://www.youtube.com/embed/mlzh94DdINg"
            };
        var random = new Random();

        // Predefined list of social media posts
        var socialMediaPosts = new List<string>
        {
            "Had an amazing time at the beach today! 🏖️ #beachday #sunshine",
            "Feeling so grateful for the support from my friends. 💖 #blessed #grateful",
            "Just finished reading a fantastic book. Highly recommend it! 📚 #bookrecommendation",
            "Check out my latest travel vlog! ✈️ #travel #vlog",
            "My new gadget just arrived and I can't wait to try it! ⚙️ #techlover #newarrival",
            "Can't believe it's already the weekend! What are your plans? 🙌 #weekendvibes",
            "Coffee and a good book, the perfect start to the day! ☕📖 #morningroutine #cozy",
            "Feeling inspired after today's workout. Time to crush some goals! 💪 #fitnessmotivation #goals",
            "Just posted a new photo on Instagram. Go check it out! 📸 #photooftheday #instagram",
            "Enjoying a peaceful evening at home. 🏡 #homelife #relaxation",
            "Had a blast at the concert last night! 🎶 #livemusic #concertvibes"
        };

        _postsFaker = new Faker<Post>()
            .RuleFor(p => p.Title, f => f.Lorem.Sentence(5)) // Random 5 words sentence
            .RuleFor(p => p.ContentType,
                f => f.PickRandom("text", "image", "video")) // Random content type
            .RuleFor(p => p.Content, (f, p) =>
            {
                return p.ContentType switch
                {
                    "text" => f.PickRandom(socialMediaPosts), // Random social media post
                    "image" => f.Image.PicsumUrl(), // Random image URL from Picsum
                    "video" => randomVideoUrls[
                        random.Next(randomVideoUrls.Count)], // Random video URL
                    _ => string.Empty
                };
            })
            .RuleFor(p => p.UserId, f => f.PickRandom(userIds)) // Random username for UserId
            .RuleFor(p => p.GroupId,
                f => f.PickRandom(groupIds.Cast<int?>()
                    .Concat(Enumerable.Repeat<int?>(null, groupIds.Count).ToList())))
            .RuleFor(p => p.CreatedAt, f => f.Date.Recent(30)) // Random date in the last 30 days
            .UseSeed(12345);
    }

    public List<Post> GeneratePosts(int count = 10)
    {
        var posts = _postsFaker.Generate(count);
        if (posts == null)
        {
            return [];
        }

        // var id = _dbContext.Users.Where(u => u.NormalizedUserName == "ANNABELLE_BERNIER79")
        //     .Select(u => u.Id).First();

        var ids = _dbContext.Users
            .Select(u => u.Id).OrderByDescending(u => u).ToList();

        posts[0].UserId = ids[0];
        posts[0].ContentType = "image";
        posts[0].Content = _faker.Image.PicsumUrl();

        posts[1].UserId = ids[1];
        posts[1].ContentType = "text";
        posts[1].Content = "🌟 Did you know? 🌟\n" +
                           "The average person spends **13 years and 2 months** of their life at work, but only **328 days** laughing! 😲\n\n" +
                           "Let’s flip the script today—take a break, laugh out loud, or share something hilarious in the comments! 😂\n\n" +
                           "💡 Here’s a joke to get you started:\n" +
                           "Why don’t skeletons fight each other?\n" +
                           "Because they don’t have the guts! 🦴\n\n" +
                           "Drop your favorite joke or funny story below! 👇✨\n\n";

        posts[2].UserId = ids[2];
        posts[2].ContentType = "video";
        posts[2].Content = "https://www.youtube.com/embed/Cqg8tQH4U8M";
        return posts;
    }
}