using ChatroomB_Backend.Controllers;
using ChatroomB_Backend.Models;
using ChatroomB_Backend.DTO;
using ChatroomB_Backend.Service;
using ChatroomB_Backend.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Azure.Core;

namespace ChatroomB_Backend_Test.ControllerTest
{
    public class AuthControllerTest
    {
        private readonly Mock<IAuthService> _mockAuthService;
        private readonly Mock<IAuthUtils> _mockAuthUtils;
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<ITokenUtils> _mockTokenUtils;
        private readonly Mock<ITokenService> _mockTokenService;
        private readonly AuthController _controller;


        public AuthControllerTest()
        {
            _mockAuthService = new Mock<IAuthService>();
            _mockAuthUtils = new Mock<IAuthUtils>();
            _mockUserService = new Mock<IUserService>();
            _mockTokenUtils = new Mock<ITokenUtils>();
            _mockTokenService = new Mock<ITokenService>();
            
            _controller = new AuthController(_mockAuthService.Object, _mockAuthUtils.Object, _mockUserService.Object, _mockTokenUtils.Object, _mockTokenService.Object);

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
            };
        }



        [Fact]
        public async Task Register_ThrowsArgumentException_WhenModelIsInvalid()
        {

            ChatroomB_Backend.DTO.RegisterRequest request = new ChatroomB_Backend.DTO.RegisterRequest(); 

            _controller.ModelState.AddModelError("Username", "Required");

            await Assert.ThrowsAsync<ArgumentException>(() => _controller.Register(request));
        }


        [Fact]
        public async Task Register_ReturnsOk_WhenRegistrationIsSuccessful()
        {
            // Arrange
            ChatroomB_Backend.DTO.RegisterRequest request = new ChatroomB_Backend.DTO.RegisterRequest
            {
                Username = "testuser",
                Password = "password",
                ProfileName = "Test User"
            };

            // Act
            IActionResult result = await _controller.Register(request);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            _mockAuthService.Verify(x => x.AddUser(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }


        [Fact]
        public async Task Login_ThrowsArgumentException_WhenModelIsInvalid()
        {

            ChatroomB_Backend.DTO.LoginRequest request = new ChatroomB_Backend.DTO.LoginRequest();

            _controller.ModelState.AddModelError("Username", "Required");

            await Assert.ThrowsAsync<ArgumentException>(() => _controller.Login(request));
        }


        [Fact]
        public async Task Login_ReturnsOk_WhenLoginIsSuccessful()
        {
            // Arrange
            RefreshToken refreshToken = new RefreshToken
            {
                TokenId = Guid.NewGuid(),
                Users = null,
                UserId = 1,
                Token = $"token_{Guid.NewGuid()}",
                ExpiredDateTime = DateTime.UtcNow.AddDays(7),
                IsDeleted = false
            };
            ChatroomB_Backend.DTO.LoginRequest request = new ChatroomB_Backend.DTO.LoginRequest
            {
                Username = "testuser",
                Password = "password",
            };

            Users user = new Users
            {
                UserId = 1,
                UserName = "testuser",
                ProfileName = "Test User 1",
                HashedPassword = "hashedPassword1",
                Salt = "salt1",
                ProfilePicture = "profilePicturePath1.jpg",
                IsDeleted = false
            };


            _mockAuthService.Setup(x => x.Authenticate(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(user);
            _mockTokenUtils.Setup(x => x.GenerateAccessToken(It.IsAny<int>(), It.IsAny<string>())).Returns("abcd");
            _mockTokenUtils.Setup(x => x.GenerateRefreshToken(It.IsAny<int>())).Returns(refreshToken);
            _mockTokenService.Setup(x => x.StoreRefreshToken(It.IsAny<RefreshToken>()));
            // Act
            IActionResult result = await _controller.Login(request);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            _mockAuthService.Verify(x => x.Authenticate(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _mockTokenService.Verify(x => x.StoreRefreshToken(It.IsAny<RefreshToken>()), Times.Once);
        }


        [Fact]
        public async Task Logout_ReturnsOk_WhenLogoutIsSuccessful()
        {
            // Arrange
            string refreshToken = "abcd";
            DefaultHttpContext context = new DefaultHttpContext();
            context.Request.Headers["X-Refresh-Token"] = refreshToken;

            _controller.ControllerContext.HttpContext = context;
            _mockTokenService.Setup(x => x.RemoveRefreshToken(It.IsAny<string>())).Returns(Task.CompletedTask);

            // Act
            IActionResult result = await _controller.Logout();

            // Assert
            Assert.IsType<OkObjectResult>(result);
            _mockTokenService.Verify(x => x.RemoveRefreshToken(It.IsAny<string>()), Times.Once);
        }


        [Fact]
        public async Task ChangePassword_ThrowsArgumentException_WhenModelIsInvalid()
        {

            PasswordChange request = new PasswordChange();

            _controller.ModelState.AddModelError("CurrentPassword", "Required");

            await Assert.ThrowsAsync<ArgumentException>(() => _controller.ChangePassword(request));
        }


        [Fact]
        public async Task ChangePassword_ReturnsOk_WhenChangePasswordIsSuccessful()
        {
            // Arrange
            PasswordChange passwordChangeModel = new PasswordChange
            {
                CurrentPassword = "currentPassword",
                NewPassword = "newPassword"
            };
            _mockAuthUtils.Setup(x => x.ExtractUserIdFromJWT(It.IsAny<ClaimsPrincipal>())).Returns(1);
            _mockAuthService.Setup(x => x.ChangePassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            // Act
            IActionResult result = await _controller.ChangePassword(passwordChangeModel);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            _mockAuthService.Verify(x => x.ChangePassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }
    }
}
