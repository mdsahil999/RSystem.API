using Microsoft.AspNetCore.Mvc;
using RSystem.API.Service.Interfaces;

namespace RSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StoryController : Controller
    {
        private readonly IStoryService _storyService;

        // Constructor injection of IStoryService
        public StoryController(IStoryService storyService)
        {
            _storyService = storyService;
        }

        /// <summary>
        /// Retrieves the latest stories from Hacker News.
        /// Response is cached for 60 seconds.
        /// </summary>
        /// <returns>A list of stories in JSON format.</returns>
        [HttpGet]
        [ResponseCache(Duration = 60)] // Cache the response for 60 seconds
        public async Task<IActionResult> GetAll()
        {
            var result = await _storyService.GetAll();
            return Ok(result);
        }
    }
}
