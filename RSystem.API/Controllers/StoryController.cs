using Microsoft.AspNetCore.Mvc;
using RSystem.API.Service.Interfaces;

namespace RSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StoryController : Controller
    {
        private readonly IStoryService _storyService;
        public StoryController(IStoryService storyService) { 
            this._storyService = storyService;
        }

        [HttpGet]
        [ResponseCache(Duration = 60)]
        public async Task<IActionResult> GetAll()
        {
            var result = await this._storyService.GetAll();
            return Ok(result);
        }
    }
}
