using ChatroomB_Backend.Controllers;
using ChatroomB_Backend.DTO;
using ChatroomB_Backend.Models;
using ChatroomB_Backend.Service;
using ChatroomB_Backend.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Moq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ChatroomB_Backend_Test.ControllerTest
{
    public class UserControllerTest
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<IAuthUtils> _mockAuthUtils;
        private readonly UsersController _controller;


        public UserControllerTest()
        {
            _mockUserService = new Mock<IUserService>();
            _mockAuthUtils = new Mock<IAuthUtils>();
            _controller = new UsersController(_mockUserService.Object, _mockAuthUtils.Object);

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
            };
        }

        [Fact]
        public async Task SearchByProfileName_WithValidName_ReturnsOkObjectResult()
        {
            // Arrange
            string profileName = "John";
            _mockAuthUtils.Setup(auth => auth.ExtractUserIdFromJWT(It.IsAny<ClaimsPrincipal>())).Returns(1);
            _mockUserService.Setup(service => service.GetByName(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(new List<UserSearchDetails>() { new UserSearchDetails() });
            List<UserSearchDetails> List = new List<UserSearchDetails> { new UserSearchDetails() };

            // Act
            IActionResult result = await _controller.SearchByProfileName(profileName);

            // Assert
            OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
            List<UserSearchDetails> returnValue = Assert.IsType<List<UserSearchDetails>>(okResult.Value);
            Assert.Single(returnValue);

            _mockUserService.Verify(x => x.GetByName(It.IsAny<string>(), It.IsAny<int>()), Times.Once);
        }


        [Fact]
        public async Task SearchByProfileName_WithEmptyName_ReturnsBadRequestObjectResult()
        {
            // Arrange
            string profileName = "";
            _mockAuthUtils.Setup(auth => auth.ExtractUserIdFromJWT(It.IsAny<ClaimsPrincipal>())).Returns(1);

            // Act
            IActionResult result = await _controller.SearchByProfileName(profileName);

            // Assert
            BadRequestObjectResult badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequest.Value);
        }


        [Fact]
        public async Task GetChatListByUserId_ShouldReturnOk_WhenDataExists()
        {
            // Arrange
            int userId = 1;
            _mockAuthUtils.Setup(x => x.ExtractUserIdFromJWT(It.IsAny<ClaimsPrincipal>())).Returns(userId);

            List<ChatlistVM> chatList = new List<ChatlistVM>
            {
                new ChatlistVM() {    ChatRoomId = 1,
                    UserChatRoomId = 100,
                    UserId = 5,
                    ProfileName = "John Doe",
                    ProfilePicture = "profilepic.jpg",
                    ChatRoomName = "Test Room",
                    RoomType = false,
                    SelectedUsers = new DataTable(),
                    InitiatedBy = 5,
                    InitiatorProfileName = "John Doe",
                    IsOnline = true }
            };

            _mockUserService.Setup(x => x.GetChatListByUserId(It.IsAny<int>())).ReturnsAsync(chatList);

            // Act
            IActionResult result = await _controller.GetChatListByUserId();

            // Assert
            OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
            IEnumerable<ChatlistVM> returnedUsers = Assert.IsAssignableFrom<IEnumerable<ChatlistVM>>(okResult.Value);
            Assert.NotNull(returnedUsers);
        }


        [Fact]
        public async Task GetFriendRequest_ReturnsOkResult_WithListOfUsers()
        {
            // Arrange
            List<Users> mockUsers = new List<Users>
        {
            new Users {
                UserId = 1,
                UserName = "testUser1",
                ProfileName = "Test User 1",
                HashedPassword = "hashedPassword1",
                Salt = "salt1",
                ProfilePicture = "profilePicturePath1.jpg",
                IsDeleted = false
            }
        };

            _mockAuthUtils.Setup(auth => auth.ExtractUserIdFromJWT(It.IsAny<ClaimsPrincipal>())).Returns(1);
            _mockUserService.Setup(service => service.GetFriendRequest(1)).ReturnsAsync(mockUsers);

            // Act
            IActionResult result = await _controller.GetFriendRequest();

            // Assert
            OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
            IEnumerable<Users> returnedUsers = Assert.IsAssignableFrom<IEnumerable<Users>>(okResult.Value);
            Assert.NotNull(returnedUsers);
            _mockUserService.Verify(x => x.GetFriendRequest(It.IsAny<int>()), Times.Once);
        }


        [Fact]
        public async Task GetUserById_UserExists_ReturnsOkResultWithUser()
        {
            // Arrange
            Users expectedUser = new Users
            {
                UserId = 1,
                UserName = "testUser1",
                ProfileName = "Test User 1",
                HashedPassword = "hashedPassword1",
                Salt = "salt1",
                ProfilePicture = "profilePicturePath1.jpg",
                IsDeleted = false
            };

            _mockAuthUtils.Setup(x => x.ExtractUserIdFromJWT(It.IsAny<ClaimsPrincipal>())).Returns(1);
            _mockUserService.Setup(s => s.GetUserById(1)).ReturnsAsync(expectedUser);

            // Act
            ActionResult<Users> result = await _controller.GetUserById();

            // Assert
            ActionResult<Users> actionResult = Assert.IsType<ActionResult<Users>>(result);
            OkObjectResult okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            Users returnedUser = Assert.IsType<Users>(okResult.Value);
            Assert.NotNull(returnedUser);
            _mockUserService.Verify(x => x.GetUserById(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task GetUserById_UserDoesNotExist_ReturnsNotFoundResult()
        {
            // Arrange
            _mockAuthUtils.Setup(x => x.ExtractUserIdFromJWT(It.IsAny<ClaimsPrincipal>())).Returns(1);
            _mockUserService.Setup(s => s.GetUserById(1)).ReturnsAsync((Users)null);

            // Act
            ActionResult<Users> result = await _controller.GetUserById();

            // Assert
            ActionResult<Users> actionResult = Assert.IsType<ActionResult<Users>>(result);
            NotFoundObjectResult notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
            Assert.Equal("User ID not found", notFoundResult.Value);
        }

        [Fact]
        public async Task UpdateProfileName_InvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            _controller.ModelState.AddModelError("Error", "Model state is invalid");
            UpdateProfileName updateProfileNameModel = new UpdateProfileName { NewProfileName = "" };

            // Act
            IActionResult result = await _controller.UpdateProfileName(updateProfileNameModel);

            // Assert
            BadRequestObjectResult badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid request data", badRequest.Value);
        }

        [Fact]
        public async Task UpdateProfileName_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            UpdateProfileName updateProfileNameModel = new UpdateProfileName { NewProfileName = "NewName" };
            _mockAuthUtils.Setup(x => x.ExtractUserIdFromJWT(It.IsAny<ClaimsPrincipal>())).Returns(1);
            _mockUserService.Setup(s => s.UpdateProfileName(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(1);

            // Act
            IActionResult result = await _controller.UpdateProfileName(updateProfileNameModel);

            // Assert
            Assert.IsType<OkResult>(result);
            _mockUserService.Verify(x => x.UpdateProfileName(It.IsAny<int>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task UpdateProfileName_UserNotFound_ReturnsNotFound()
        {
            // Arrange
            UpdateProfileName updateProfileNameModel = new UpdateProfileName { NewProfileName = "NewName" };
            _mockAuthUtils.Setup(x => x.ExtractUserIdFromJWT(It.IsAny<ClaimsPrincipal>())).Returns(1);
            _mockUserService.Setup(s => s.UpdateProfileName(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(0);

            // Act
            IActionResult result = await _controller.UpdateProfileName(updateProfileNameModel);

            // Assert
            NotFoundObjectResult notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("User ID not found or update failed.", notFoundResult.Value);
            _mockUserService.Verify(x => x.UpdateProfileName(It.IsAny<int>(), It.IsAny<string>()), Times.Once);
        }


        [Fact]
        public async Task UpdateProfilePicture_ReturnsBadRequest_WhenFileIsNotValid()
        {
            // Arrange
            IFormFile file = null;

            _mockAuthUtils.Setup(x => x.ExtractUserIdFromJWT(It.IsAny<ClaimsPrincipal>())).Returns(1);

            // Act
            IActionResult result = await _controller.UpdateProfilePicture(file);

            // Assert
            BadRequestObjectResult badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("File is not provided or empty.", badRequest.Value);
        }

        [Fact]
        public async Task UpdateProfilePicture_ReturnsBadRequest_WhenFileIsTooLarge()
        {
            // Arrange
            int largeFileSize = 8 * 1024 * 1024;
            Mock<IFormFile> fileMock = new Mock<IFormFile>();
            fileMock.Setup(_ => _.Length).Returns(largeFileSize);

            // Act
            IActionResult result = await _controller.UpdateProfilePicture(fileMock.Object);

            // Assert
            BadRequestObjectResult badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("The file is too large. Please upload an image that is 7MB or smaller.", badRequest.Value);
        }

        [Fact]
        public async Task UpdateProfilePicture_ReturnsBadRequest_WhenFileIsNotValidImage()
        {
            // Arrange
            string fileName = "testImage.jpg";
            Mock<IFormFile> fileMock = new Mock<IFormFile>();
            string content = "Hello, World!";
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(content));
            string contentType = "image/plain";

            fileMock.Setup(_ => _.FileName).Returns(fileName);
            fileMock.Setup(_ => _.Length).Returns(ms.Length);
            fileMock.Setup(_ => _.ContentType).Returns(contentType);
            fileMock.Setup(_ => _.CopyToAsync(It.IsAny<Stream>(), It.IsAny<System.Threading.CancellationToken>()))
                    .Returns((Stream stream, System.Threading.CancellationToken token) => ms.CopyToAsync(stream));
            _mockAuthUtils.Setup(x => x.ExtractUserIdFromJWT(It.IsAny<ClaimsPrincipal>())).Returns(1);
            _mockUserService.Setup(x => x.UpdateProfilePicture(It.IsAny<int>(), It.IsAny<byte[]>(), It.IsAny<string>()))
                            .ReturnsAsync(-1);

            // Act
            IActionResult result = await _controller.UpdateProfilePicture(fileMock.Object);

            // Assert
            BadRequestObjectResult badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("The uploaded file is not a valid image.", badRequest.Value);
            _mockUserService.Verify(x => x.UpdateProfilePicture(It.IsAny<int>(), It.IsAny<byte[]>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task UpdateProfilePicture_ReturnsBadRequest_WhenFailUpdateImage()
        {
            // Arrange
            string fileName = "testImage.jpg";
            Mock<IFormFile> fileMock = new Mock<IFormFile>();
            string content = "Hello, World!";
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(content));
            string contentType = "image/plain";

            fileMock.Setup(_ => _.FileName).Returns(fileName);
            fileMock.Setup(_ => _.Length).Returns(ms.Length);
            fileMock.Setup(_ => _.ContentType).Returns(contentType);
            fileMock.Setup(_ => _.CopyToAsync(It.IsAny<Stream>(), It.IsAny<System.Threading.CancellationToken>()))
                    .Returns((Stream stream, System.Threading.CancellationToken token) => ms.CopyToAsync(stream));
            _mockAuthUtils.Setup(x => x.ExtractUserIdFromJWT(It.IsAny<ClaimsPrincipal>())).Returns(1);
            _mockUserService.Setup(x => x.UpdateProfilePicture(It.IsAny<int>(), It.IsAny<byte[]>(), It.IsAny<string>()))
                            .ReturnsAsync(0);

            // Act
            IActionResult result = await _controller.UpdateProfilePicture(fileMock.Object);

            // Assert
            NotFoundObjectResult notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Failed to update the profile picture.", notFoundResult.Value);
            _mockUserService.Verify(x => x.UpdateProfilePicture(It.IsAny<int>(), It.IsAny<byte[]>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task UpdateProfilePicture_ReturnsOk_WhenFileIsValid()
        {
            // Arrange
            string fileName = "testImage.jpg";
            Mock<IFormFile> fileMock = new Mock<IFormFile>();
            string content = "Hello, World!";
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(content));
            string contentType = "image/jpeg";

            fileMock.Setup(_ => _.FileName).Returns(fileName);
            fileMock.Setup(_ => _.Length).Returns(ms.Length);
            fileMock.Setup(_ => _.ContentType).Returns(contentType);
            fileMock.Setup(_ => _.CopyToAsync(It.IsAny<Stream>(), It.IsAny<System.Threading.CancellationToken>()))
                    .Returns((Stream stream, System.Threading.CancellationToken token) => ms.CopyToAsync(stream));
            _mockAuthUtils.Setup(x => x.ExtractUserIdFromJWT(It.IsAny<ClaimsPrincipal>())).Returns(1);
            _mockUserService.Setup(x => x.UpdateProfilePicture(It.IsAny<int>(), It.IsAny<byte[]>(), It.IsAny<string>()))
                            .ReturnsAsync(1);

            // Act
            IActionResult result = await _controller.UpdateProfilePicture(fileMock.Object);

            // Assert
            OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
            _mockUserService.Verify(x => x.UpdateProfilePicture(It.IsAny<int>(), It.IsAny<byte[]>(), It.IsAny<string>()), Times.Once);
        }


        [Fact]
        public async Task DeleteUser_UserNotFound_ReturnsNotFound()
        {
            // Arrange
            _mockAuthUtils.Setup(x => x.ExtractUserIdFromJWT(It.IsAny<ClaimsPrincipal>())).Returns(1);
            _mockUserService.Setup(s => s.DeleteUser(It.IsAny<int>())).ReturnsAsync(0);

            // Act
            IActionResult result = await _controller.DeleteUser();

            // Assert
            NotFoundObjectResult notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("User ID not found.", notFoundResult.Value);
            _mockUserService.Verify(x => x.DeleteUser(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task DeleteUser_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            _mockAuthUtils.Setup(x => x.ExtractUserIdFromJWT(It.IsAny<ClaimsPrincipal>())).Returns(1);
            _mockUserService.Setup(s => s.DeleteUser(It.IsAny<int>())).ReturnsAsync(1);

            // Act
            IActionResult result = await _controller.DeleteUser();

            // Assert
            Assert.IsType<OkResult>(result);
            _mockUserService.Verify(x => x.DeleteUser(It.IsAny<int>()), Times.Once);
        }

    }
}
