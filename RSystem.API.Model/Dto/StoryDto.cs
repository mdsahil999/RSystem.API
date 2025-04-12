using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSystem.API.Model.Dto
{
    public class StoryDto
    {
        public string By { get; set; }
        public int Descendants { get; set; }
        public int Id { get; set; }
        public int Score { get; set; }
        public long Time { get; set; } // Unix timestamp
        public string Title { get; set; }
        public string Type { get; set; }
        public string Url { get; set; }
    }
}
