namespace RSystem.API.Model.Dto
{
    /// <summary>
    /// Represents a story retrieved from Hacker News.
    /// </summary>
    public class StoryDto
    {
        /// <summary>
        /// The unique identifier of the story.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The title of the story.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The URL the story links to.
        /// </summary>
        public string Url { get; set; }
    }
}
