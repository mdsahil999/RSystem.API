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
using Microsoft.Extensions.Configuration;

namespace RSystem.MsTest.Test
{
    [TestClass]
    public class StoryServiceTests
    {
        private const string BaseUrl = "https://hacker-news.firebaseio.com/v0/";
        private const string MaxStories = "200";

        /// <summary>
        /// Creates a mock IConfiguration with expected values.
        /// </summary>
        private IConfiguration CreateMockConfiguration()
        {
            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(c => c["HackerNewsSettings:BaseUrl"]).Returns(BaseUrl);
            mockConfig.Setup(c => c["HackerNewsSettings:MaxStories"]).Returns(MaxStories);
            return mockConfig.Object;
        }

        /// <summary>
        /// Creates a mock HttpClient that returns predefined responses for specific URLs.
        /// </summary>
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

            return new HttpClient(handlerMock.Object);
        }

        [TestMethod]
        public async Task GetAll_Returns_Only_Valid_Stories_With_Urls()
        {
            // Arrange
            var storyIds = new List<int> { 1001, 1002, 1003 };
            var idUrl = $"{BaseUrl}newstories.json";

            var fakeResponses = new Dictionary<string, string>
            {
                [idUrl] = JsonSerializer.Serialize(storyIds),
                [$"{BaseUrl}item/1001.json"] = JsonSerializer.Serialize(new StoryDto { Id = 1001, Url = "https://example.com/1", Title = "Story 1" }),
                [$"{BaseUrl}item/1002.json"] = JsonSerializer.Serialize(new StoryDto { Id = 1002, Url = null, Title = "Story 2" }),
                [$"{BaseUrl}item/1003.json"] = "null"
            };

            var httpClient = CreateMockHttpClient(fakeResponses);
            var config = CreateMockConfiguration();
            var service = new StoryService(httpClient, config);

            // Act
            var result = await service.GetAll();

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(1001, result.First().Id);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "Failed to retrieve Hacker News stories.")]
        public async Task GetAll_Throws_Exception_When_Ids_Not_Fetched()
        {
            var idUrl = $"{BaseUrl}newstories.json";

            var fakeResponses = new Dictionary<string, string>
            {
                [idUrl] = ""
            };

            var httpClient = CreateMockHttpClient(fakeResponses);
            var config = CreateMockConfiguration();
            var service = new StoryService(httpClient, config);

            await service.GetAll();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "No story IDs found from Hacker News.")]
        public async Task GetAll_Throws_Exception_When_Id_List_Is_Empty()
        {
            var idUrl = $"{BaseUrl}newstories.json";

            var fakeResponses = new Dictionary<string, string>
            {
                [idUrl] = "[]"
            };

            var httpClient = CreateMockHttpClient(fakeResponses);
            var config = CreateMockConfiguration();
            var service = new StoryService(httpClient, config);

            await service.GetAll();
        }
    }
}
