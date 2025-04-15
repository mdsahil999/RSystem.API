using RSystem.API.Model.Dto;

namespace RSystem.API.Service.Interfaces
{
    /// <summary>
    /// Defines the contract for fetching stories from an external service.
    /// </summary>
    public interface IStoryService
    {
        /// <summary>
        /// Retrieves a list of the latest stories.
        /// </summary>
        /// <returns>A task representing the asynchronous operation. 
        /// The task result contains a list of <see cref="StoryDto"/> objects.</returns>
        Task<List<StoryDto>> GetAll();
    }
}
