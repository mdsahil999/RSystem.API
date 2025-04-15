using RSystem.API.Model.Dto;
using RSystem.API.Service.Interfaces;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace RSystem.API.Service.Services
{
    public class StoryService : IStoryService
    {
        private readonly HttpClient _httpClient;

        // Base URL for Hacker News API
        private readonly string _baseUrl;

        // Maximum number of stories to fetch
        private readonly int _maxStories;

        public StoryService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["HackerNewsSettings:BaseUrl"] ?? throw new ArgumentNullException("BaseUrl not found");
            _maxStories = int.Parse(configuration["HackerNewsSettings:MaxStories"] ?? "100"); // fallback to 100

        }

        // Fetches the latest stories from Hacker News
        public async Task<List<StoryDto>> GetAll()
        {
            try
            {
                // Step 1: Get the list of story IDs
                var ids = await FetchStoryIdsAsync();

                // Check if any IDs were returned
                if (ids == null || ids.Count == 0)
                    throw new Exception("No story IDs found from Hacker News.");

                // Step 2: Fetch story details for the first 200 stories
                var stories = await FetchStoriesAsync(ids.Take(_maxStories));

                // Step 3: Filter out null or invalid stories (e.g., missing URL)
                return stories
                    .Where(story => story != null && !string.IsNullOrEmpty(story.Url))
                    .ToList();
            }
            catch (Exception ex)
            {
                // Log error to console and rethrow a generic exception
                Console.WriteLine($"Error fetching stories: {ex.Message}");
                throw new Exception("Failed to retrieve Hacker News stories.", ex);
            }
        }

        // Fetches the list of new story IDs from Hacker News
        private async Task<List<int>?> FetchStoryIdsAsync()
        {
            var response = await _httpClient.GetStringAsync($"{_baseUrl}newstories.json");
            return JsonSerializer.Deserialize<List<int>>(response);
        }

        // Fetches the story details for the given list of IDs
        private async Task<IEnumerable<StoryDto?>> FetchStoriesAsync(IEnumerable<int> ids)
        {
            // Use parallel requests to fetch each story
            var tasks = ids.Select(async id =>
            {
                try
                {
                    // Try to fetch story data; skip if an error occurs
                    return await _httpClient.GetFromJsonAsync<StoryDto>($"{_baseUrl}item/{id}.json");
                }
                catch
                {
                    return null; // Skip failed fetches silently
                }
            });

            return await Task.WhenAll(tasks);
        }
    }
}
