using RSystem.API.Model.Dto;

namespace RSystem.API.Service.Interfaces
{
    public interface IStoryService
    {
        Task<List<StoryDto>> GetAll();
    }
}
