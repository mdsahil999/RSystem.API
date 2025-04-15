using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Http;
using Moq;
using Moq.Protected;
using System.Net;
using System.Linq;
using System.Text.Json;
using RSystem.API.Model.Dto;
using RSystem.API.Service.Services;

namespace RSystem.MsTest.Test
{
    [TestClass]
    public class StoryServiceTests
    {
        /// <summary>
        /// Creates a mock HttpClient that returns predefined responses for specific URLs.
        /// </summary>
        /// <param name="responses">Dictionary mapping request URLs to response content.</param>
        private HttpClient CreateMockHttpClient(Dictionary<string, string> responses)
        {
            var handlerMock = new Mock<HttpMessageHandler>();

            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Returns<HttpRequestMessage, CancellationToken>((request, token) =>
                {
                    string url = request.RequestUri.ToString();

                    if (responses.TryGetValue(url, out var content))
                    {
                        return Task.FromResult(new HttpResponseMessage
                        {
                            StatusCode = HttpStatusCode.OK,
                            Content = new StringContent(content)
                        });
                    }

                    return Task.FromResult(new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.NotFound
                    });
                });

            return new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("https://hacker-news.firebaseio.com/")
            };
        }

        /// <summary>
        /// Verifies that GetAll returns only valid stories that have a non-null URL.
        /// </summary>
        [TestMethod]
        public async Task GetAll_Returns_Only_Valid_Stories_With_Urls()
        {
            // Arrange
            var storyIds = new List<int> { 1001, 1002, 1003 };
            var idUrl = "https://hacker-news.firebaseio.com/v0/newstories.json";

            var fakeResponses = new Dictionary<string, string>
            {
                [idUrl] = JsonSerializer.Serialize(storyIds),
                [$"https://hacker-news.firebaseio.com/v0/item/1001.json"] = JsonSerializer.Serialize(new StoryDto { Id = 1001, Url = "https://example.com/1", Title = "Story 1" }),
                [$"https://hacker-news.firebaseio.com/v0/item/1002.json"] = JsonSerializer.Serialize(new StoryDto { Id = 1002, Url = null, Title = "Story 2" }), // Invalid story (no URL)
                [$"https://hacker-news.firebaseio.com/v0/item/1003.json"] = "null" // Simulate null response
            };

            var httpClient = CreateMockHttpClient(fakeResponses);
            var service = new StoryService(httpClient);

            // Act
            var result = await service.GetAll();

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(1001, result.First().Id);
        }

        /// <summary>
        /// Verifies that an exception is thrown when the story ID list cannot be fetched (empty response).
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(Exception), "Failed to retrieve Hacker News stories.")]
        public async Task GetAll_Throws_Exception_When_Ids_Not_Fetched()
        {
            // Arrange
            var idUrl = "https://hacker-news.firebaseio.com/v0/newstories.json";

            var fakeResponses = new Dictionary<string, string>
            {
                [idUrl] = "" // Empty response simulates failure to fetch IDs
            };

            var httpClient = CreateMockHttpClient(fakeResponses);
            var service = new StoryService(httpClient);

            // Act
            await service.GetAll();

            // Assert is handled by [ExpectedException]
        }

        /// <summary>
        /// Verifies that an exception is thrown when the story ID list is empty.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(Exception), "No story IDs found from Hacker News.")]
        public async Task GetAll_Throws_Exception_When_Id_List_Is_Empty()
        {
            // Arrange
            var idUrl = "https://hacker-news.firebaseio.com/v0/newstories.json";

            var fakeResponses = new Dictionary<string, string>
            {
                [idUrl] = "[]" // Simulates empty ID list
            };

            var httpClient = CreateMockHttpClient(fakeResponses);
            var service = new StoryService(httpClient);

            // Act
            await service.GetAll();

            // Assert is handled by [ExpectedException]
        }
    }
}
