using Google.Apis.Services;
using Google.Apis.YouTube.v3;

namespace Studentescu.Utils;

public class YouTubeRandomVideos
{
    public async Task<List<string>> GetRandomCatYouTubeEmbedUrlsAsync(string? apiKey,
        int numberOfVideos = 50)
    {
        var youtubeService = new YouTubeService(new BaseClientService.Initializer
        {
            ApiKey = apiKey,
            ApplicationName = GetType().ToString()
        });

        // Search for cat-related videos
        var searchRequest = youtubeService.Search.List("snippet");
        searchRequest.Q = "cat"; // Search for "cat" videos
        searchRequest.MaxResults = numberOfVideos; // Limit to 15 videos

        // Execute the search request
        var searchResponse = await searchRequest.ExecuteAsync();

        var videoUrls = new List<string>();

        // Loop through the search results and check if each video is available
        foreach (var video in searchResponse.Items)
        {
            try
            {
                // Attempt to get video details (this can help identify unavailable videos)
                var videoRequest = youtubeService.Videos.List("status");
                videoRequest.Id = video.Id.VideoId;
                var videoDetails = await videoRequest.ExecuteAsync();

                // Check if the video is public (not private or unavailable)
                var isAvailable = videoDetails.Items.FirstOrDefault()?.Status?.PrivacyStatus ==
                                  "public";

                if (isAvailable)
                {
                    var embedUrl = $"https://www.youtube.com/embed/{video.Id.VideoId}";
                    videoUrls.Add(embedUrl);
                }
            }
            catch (Exception ex)
            {
                // Handle any errors (e.g., video not found, private, etc.)
                Console.WriteLine($"Error fetching video details: {ex.Message}");
            }
        }

        return videoUrls;
    }
}