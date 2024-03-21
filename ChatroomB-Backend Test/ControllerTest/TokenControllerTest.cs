using Azure.Core;
using ChatroomB_Backend.Controllers;
using ChatroomB_Backend.Models;
using ChatroomB_Backend.Service;
using ChatroomB_Backend.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatroomB_Backend_Test.ControllerTest
{
    public class TokenControllerTest
    {
        private readonly Mock<ITokenService> _mockTokenService;
        private readonly Mock<ITokenUtils> _mockTokenUtils;
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly TokenController _controller;

        public TokenControllerTest()
        {
            _mockTokenService = new Mock<ITokenService>();
            _mockTokenUtils = new Mock<ITokenUtils>();
            _mockConfig = new Mock<IConfiguration>();

            _controller = new TokenController(_mockTokenService.Object, _mockTokenUtils.Object, _mockConfig.Object);

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
            };
        }

        [Fact]
        public async Task RenewToken_ThrowsArgumentException_WhenInValidRefreshToken()
        {
            //Arrange
            string refreshToken = "";
            DefaultHttpContext context = new DefaultHttpContext();
            context.Request.Headers["X-Refresh-Token"] = refreshToken;

            // Act
            ArgumentException exception = await Assert.ThrowsAsync<ArgumentException>(() => _controller.RenewToken());

            // Assert
            Assert.Equal("Refresh token is required", exception.Message);
        }

        [Fact]
        public async Task RenewToken_ThrowsUnauthorizedAccessException_WhenUserInfoIsMissing()
        {
            // Arrange is done above
            _controller.ControllerContext.HttpContext.Request.Headers["X-Refresh-Token"] = "abcd";

            // Act
            UnauthorizedAccessException exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await _controller.RenewToken());

            // Assert
            Assert.Equal("User information is missing in the request context", exception.Message);
        }

        [Fact]
        public async Task RenewToken_ReturnsNewAccessToken()
        {
            // Arrange
            _controller.ControllerContext.HttpContext.Request.Headers["X-Refresh-Token"] = "abcd";
            _controller.ControllerContext.HttpContext.Items["UserId"] = 1;
            _controller.ControllerContext.HttpContext.Items["Username"] = "testUser";

            _mockTokenService.Setup(x => x.ValidateRefreshToken(It.IsAny<string>(), It.IsAny<int>())).Returns(Task.CompletedTask);
            _mockTokenService.Setup(x => x.UpdateRefreshToken(It.IsAny<string>())).Returns(Task.CompletedTask);
            _mockTokenUtils.Setup(x => x.GenerateAccessToken(It.IsAny<int>(), It.IsAny<string>())).Returns("abcd");

            // Act
            IActionResult result = await _controller.RenewToken();

            // Assert
            Assert.IsType<OkObjectResult>(result);
            _mockTokenService.Verify(x => x.ValidateRefreshToken(It.IsAny<string>(), It.IsAny<int>()), Times.Once);
            _mockTokenService.Verify(x => x.UpdateRefreshToken(It.IsAny<string>()), Times.Once);
            _mockTokenUtils.Verify(x => x.GenerateAccessToken(It.IsAny<int>(), It.IsAny<string>()), Times.Once);
        }



        [Fact]
        public async Task ValidateRefreshToken_ThrowsArgumentException_WhenInValidRefreshToken()
        {
            //Arrange
            string refreshToken = "";
            DefaultHttpContext context = new DefaultHttpContext();
            context.Request.Headers["X-Refresh-Token"] = refreshToken;

            // Act
            ArgumentException exception = await Assert.ThrowsAsync<ArgumentException>(() => _controller.ValidateRefreshToken());

            // Assert
            Assert.Equal("Refresh token is required", exception.Message);
        }

        [Fact]
        public async Task ValidateRefreshToken_ThrowsUnauthorizedAccessException_WhenUserInfoIsMissing()
        {
            // Arrange is done above
            _controller.ControllerContext.HttpContext.Request.Headers["X-Refresh-Token"] = "abcd";

            // Act
            UnauthorizedAccessException exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await _controller.ValidateRefreshToken());

            // Assert
            Assert.Equal("User information is missing in the request context", exception.Message);
        }

        [Fact]
        public async Task ValidateRefreshToken_ReturnsNewAccessToken_WhenCalledWithValidRefreshToken()
        {
            // Arrange
            _controller.ControllerContext.HttpContext.Request.Headers["X-Refresh-Token"] = "abcd";
            _controller.ControllerContext.HttpContext.Items["UserId"] = 1;

            _mockTokenService.Setup(x => x.ValidateRefreshToken(It.IsAny<string>(), It.IsAny<int>())).Returns(Task.CompletedTask);

            // Act
            IActionResult result = await _controller.ValidateRefreshToken();

            // Assert
            Assert.IsType<OkResult>(result);
            _mockTokenService.Verify(x => x.ValidateRefreshToken(It.IsAny<string>(), It.IsAny<int>()), Times.Once);
        }

    }
}
