namespace RSystem.API.Model.Dto
{
    /// <summary>
    /// Represents a story retrieved from Hacker News.
    /// </summary>
    public class StoryDto
    {
        /// <summary>
        /// The username of the author who submitted the story.
        /// </summary>
        public string By { get; set; }

        /// <summary>
        /// The total number of comments on the story.
        /// </summary>
        public int Descendants { get; set; }

        /// <summary>
        /// The unique identifier of the story.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The score (upvotes) of the story.
        /// </summary>
        public int Score { get; set; }

        /// <summary>
        /// The Unix timestamp when the story was created.
        /// </summary>
        public long Time { get; set; }

        /// <summary>
        /// The title of the story.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The type of the item (e.g., "story", "comment").
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The URL the story links to.
        /// </summary>
        public string Url { get; set; }
    }
}
