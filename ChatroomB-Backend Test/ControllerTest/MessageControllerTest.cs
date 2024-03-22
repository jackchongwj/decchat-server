using ChatroomB_Backend.Controllers;
using ChatroomB_Backend.DTO;
using ChatroomB_Backend.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatroomB_Backend_Test.ControllerTest
{
    public class MessageControllerTest
    {
        private readonly Mock<IMessageService> _mockMessageService;
        private readonly Mock<IRabbitMQServices> _mockRabbitMQService;
        private readonly Mock<IApplicationServices> _mockApplicationServices;
        private readonly MessagesController _controller;

        public MessageControllerTest()
        {
            _mockMessageService = new Mock<IMessageService>();
            _mockRabbitMQService = new Mock<IRabbitMQServices>();
            _mockApplicationServices = new Mock<IApplicationServices>();
            _controller = new MessagesController(_mockMessageService.Object, _mockRabbitMQService.Object, _mockApplicationServices.Object);

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
            };
        }

        [Fact]
        public async Task AddMessage_ShouldReturnBadRequest_WhenMessageContentIsEmpty()
        {
            // Arrange
            _controller.ControllerContext.HttpContext.Request.Form = new FormCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
            {
                { "message", "" } 
            });
            IFormFile? file = null;

            // Act
            var result = await _controller.AddMessage(file);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("The message content is missing.", badRequestResult.Value);
        }

        [Fact]
        public async Task AddMessage_ShouldReturnBadRequest_WhenMessageContentCannotBeParsed()
        {
            // Arrange
            string invalidMessageJson = "invalid json";
            FormCollection formCollection = new FormCollection(new Dictionary<string, StringValues>
            {
                { "message", invalidMessageJson }
            });

            _controller.ControllerContext.HttpContext.Request.Form = formCollection;

            // Act
            var result = await _controller.AddMessage(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("The message content could not be parsed.", badRequestResult.Value);
        }

        [Fact]
        public async Task AddMessage_ShouldPublishMessage_WhenContentIsValidAndNoFile()
        {
            // Arrange
            ChatRoomMessage message = new ChatRoomMessage {
                MessageId = 1,
                Content = "Hello, this is a sample message.",
                UserChatRoomId = 10,
                TimeStamp = DateTime.Now,
                ResourceUrl = "http://example.com/resource.jpg",
                IsDeleted = false,
                ChatRoomId = 5,
                UserId = 2,
                ProfileName = "John Doe",
                ProfilePicture = "http://example.com/profile.jpg"
            };

            string validMessageJson = JsonConvert.SerializeObject(message);
            FormCollection formCollection = new FormCollection(new Dictionary<string, StringValues>
            {
                { "message", validMessageJson }
            });

            _controller.ControllerContext.HttpContext.Request.Form = formCollection;

            // Act
            var result = await _controller.AddMessage(null);

            // Assert
            Assert.IsType<OkResult>(result);
            _mockRabbitMQService.Verify(x => x.PublishMessage(
                It.Is<FileMessage>(fm =>
                    fm.Message!.Content == message.Content &&
                    fm.FileByte == null
                )), Times.Once);

        }

        [Fact]
        public async Task AddMessage_ShouldPublishMessage_WhenContentIsValidAndFileIsAttached()
        {
            // Arrange
            ChatRoomMessage message = new ChatRoomMessage
            {
                MessageId = 1,
                Content = "Hello, this is a sample message.",
                UserChatRoomId = 10,
                TimeStamp = DateTime.Now,
                ResourceUrl = "http://example.com/resource.jpg",
                IsDeleted = false,
                ChatRoomId = 5,
                UserId = 2,
                ProfileName = "John Doe",
                ProfilePicture = "http://example.com/profile.jpg"
            };
            var validMessageJson = JsonConvert.SerializeObject(message);
            var fileMock = new Mock<IFormFile>();
            var fileName = "test.txt";
            var fileContent = "Hello File";
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));
            fileMock.Setup(_ => _.FileName).Returns(fileName);
            fileMock.Setup(_ => _.ContentType).Returns("text/plain");
            fileMock.Setup(_ => _.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                    .Callback<Stream, CancellationToken>((stream, token) => ms.CopyTo(stream));

            var formCollection = new FormCollection(new Dictionary<string, StringValues>
        {
            { "message", validMessageJson }
        }, new FormFileCollection { fileMock.Object });

            _controller.ControllerContext.HttpContext.Request.Form = formCollection;

            // Act
            var result = await _controller.AddMessage(fileMock.Object);

            // Assert
            Assert.IsType<OkResult>(result);
            _mockRabbitMQService.Verify(x => x.PublishMessage(It.Is<FileMessage>(fm => fm.Message == message && fm.FileName == fileName && fm.FileType == "text")), Times.Once);
        }



    }
}
