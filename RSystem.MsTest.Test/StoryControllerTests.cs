﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RSystem.API.Controllers;
using RSystem.API.Model.Dto;
using RSystem.API.Service.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RSystem.MsTest.Test
{
    [TestClass]
    public class StoryControllerTests
    {
        private Mock<IStoryService> _storyServiceMock;
        private StoryController _controller;

        /// <summary>
        /// Initializes test dependencies before each test runs.
        /// </summary>
        [TestInitialize]
        public void Setup()
        {
            _storyServiceMock = new Mock<IStoryService>();
            _controller = new StoryController(_storyServiceMock.Object);
        }

        /// <summary>
        /// Test to verify that the GetAll method returns an Ok result
        /// with the expected list of stories.
        /// </summary>
        [TestMethod]
        public async Task GetAll_ReturnsOkResult_WithListOfStories()
        {
            // Arrange: Set up expected result
            var expectedStories = new List<StoryDto>
            {
                new StoryDto { Id = 1, Title = "Sample Story", Url = "https://example.com" }
            };

            _storyServiceMock.Setup(service => service.GetAll())
                .ReturnsAsync(expectedStories);

            // Act: Call the controller method
            var actionResult = await _controller.GetAll();

            // Assert: Verify the response is Ok (200) with correct data
            var okResult = actionResult as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var actualStories = okResult.Value as List<StoryDto>;
            Assert.IsNotNull(actualStories);
            Assert.AreEqual(1, actualStories.Count);
            Assert.AreEqual(expectedStories[0].Id, actualStories[0].Id);
        }
    }
}
