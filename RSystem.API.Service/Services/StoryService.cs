using RSystem.API.Model.Dto;
using RSystem.API.Service.Interfaces;
using System.Net.Http.Json;
using System.Text.Json;

namespace RSystem.API.Service.Services
{
    public class StoryService: IStoryService
    {
        private readonly HttpClient _httpClient;
        public StoryService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<StoryDto>> GetAll()
        {
            try
            {
                var idsResponse = await _httpClient.GetStringAsync("https://hacker-news.firebaseio.com/v0/newstories.json");
                var ids = JsonSerializer.Deserialize<List<int>>(idsResponse);

                if (ids == null || ids.Count == 0)
                    throw new Exception("No story IDs found from Hacker News.");

                var tasks = ids.Select(async id =>
                {
                    try
                    {
                        return await _httpClient.GetFromJsonAsync<StoryDto>(
                            $"https://hacker-news.firebaseio.com/v0/item/{id}.json");
                    }
                    catch
                    {
                        return null;
                    }
                });

                var stories = await Task.WhenAll(tasks);
                var filteredStories = stories
                    .Where(story => story != null && !string.IsNullOrEmpty(story.Url))
                    .ToList();
                return filteredStories;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching stories: {ex.Message}");
                throw new Exception("Failed to retrieve Hacker News stories.", ex);
            }
        }


    }
}
