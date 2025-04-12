using RSystem.API.Model.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSystem.API.Service.Interfaces
{
    public interface IStoryService
    {
        Task<List<StoryDto>> GetAll();
    }
}
