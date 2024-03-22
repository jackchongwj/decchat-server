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
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ChatroomB_Backend_Test.ControllerTest
{
    public class ChatRoomControllerTest
    {
        private readonly Mock<IChatRoomService> _mockChatRoomService;
        private readonly Mock<IAuthUtils> _mockAuthUtils;
        private readonly ChatRoomController _controller;

        public ChatRoomControllerTest()
        {
            _mockChatRoomService = new Mock<IChatRoomService>();
            _mockAuthUtils = new Mock<IAuthUtils>();
            _controller = new ChatRoomController(_mockChatRoomService.Object, _mockAuthUtils.Object);

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
            };
        }


        [Fact]
        public async Task UpdateGroupPicture_ReturnsBadRequest_WhenFileIsNotValid()
        {
            // Arrange
            IFormFile file = null;
            string chatRoomId = "1";
            _mockAuthUtils.Setup(x => x.ExtractUserIdFromJWT(It.IsAny<ClaimsPrincipal>())).Returns(1);

            // Act
            IActionResult result = await _controller.UpdateGroupPicture(file, chatRoomId);

            // Assert
            BadRequestObjectResult badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("File is not provided or empty.", badRequest.Value);
        }

        [Fact]
        public async Task UpdateGroupPicture_ReturnsBadRequest_WhenFileIsTooLarge()
        {
            // Arrange
            string chatRoomId = "1";
            int largeFileSize = 8 * 1024 * 1024;
            Mock<IFormFile> fileMock = new Mock<IFormFile>();
            fileMock.Setup(_ => _.Length).Returns(largeFileSize);

            // Act
            IActionResult result = await _controller.UpdateGroupPicture(fileMock.Object, chatRoomId);

            // Assert
            BadRequestObjectResult badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("The file is too large. Please upload an image that is 7MB or smaller.", badRequest.Value);
        }

        [Fact]
        public async Task UpdateGroupPicture_ReturnsBadRequest_WhenFileIsNotValidImage()
        {
            // Arrange
            string chatRoomId = "1";
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
            _mockChatRoomService.Setup(x => x.UpdateGroupPicture(It.IsAny<int>(), It.IsAny<byte[]>(), It.IsAny<string>()))
                            .ReturnsAsync(-1);

            // Act
            IActionResult result = await _controller.UpdateGroupPicture(fileMock.Object, chatRoomId);

            // Assert
            BadRequestObjectResult badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("The uploaded file is not a valid image.", badRequest.Value);
            _mockChatRoomService.Verify(x => x.UpdateGroupPicture(It.IsAny<int>(), It.IsAny<byte[]>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task UpdateGroupPicture_ReturnsBadRequest_WhenFailUpdateImage()
        {
            //Arrange
            string chatRoomId = "1";
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
            _mockChatRoomService.Setup(x => x.UpdateGroupPicture(It.IsAny<int>(), It.IsAny<byte[]>(), It.IsAny<string>()))
                            .ReturnsAsync(0);

            //Act
           IActionResult result = await _controller.UpdateGroupPicture(fileMock.Object,chatRoomId);

            //Assert
           NotFoundObjectResult notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
           Assert.Equal("Failed to update the group picture.", notFoundResult.Value);
           _mockChatRoomService.Verify(x => x.UpdateGroupPicture(It.IsAny<int>(), It.IsAny<byte[]>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task UpdateProfilePicture_ReturnsOk_WhenFileIsValid()
        {

            // Arrange
            string chatRoomId = "1";
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
            _mockChatRoomService.Setup(x => x.UpdateGroupPicture(It.IsAny<int>(), It.IsAny<byte[]>(), It.IsAny<string>()))
                            .ReturnsAsync(1);

            // Act
            IActionResult result = await _controller.UpdateGroupPicture(fileMock.Object, chatRoomId);

            // Assert
            OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
            _mockChatRoomService.Verify(x => x.UpdateGroupPicture(It.IsAny<int>(), It.IsAny<byte[]>(), It.IsAny<string>()), Times.Once);
        }


        [Fact]
        public async Task GetGroupMembers_ReturnsOk_WithGroupMembers()
        {
            // Arrange
            int chatRoomId = 1;
            List<GroupMember> groupMembers = new List<GroupMember>
            {
                new GroupMember {        
                    ChatRoomId = 1,
                    UserId = 101,
                    ProfileName = "UserOne",
                    ProfilePicture = "userone_pic.jpg"
                }
            };
            _mockAuthUtils.Setup(x => x.ExtractUserIdFromJWT(It.IsAny<ClaimsPrincipal>())).Returns(1);
            _mockChatRoomService.Setup(s => s.RetrieveGroupMemberByChatroomId(chatRoomId, 1))
                                .ReturnsAsync(groupMembers);

            // Act
            var result = await _controller.GetGroupMembers(chatRoomId);

            // Assert
            OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
            IEnumerable<GroupMember> returnedUsers = Assert.IsAssignableFrom<IEnumerable<GroupMember>>(okResult.Value);
            Assert.NotNull(returnedUsers);
            _mockChatRoomService.Verify(x => x.RetrieveGroupMemberByChatroomId(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
        }


        [Fact]
        public async Task CreateGroup_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("error", "some error");

            // Act
            IActionResult result = await _controller.CreateGroup(new CreateGroupVM());

            // Assert
            BadRequestObjectResult badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid request data", badRequest.Value);
        }

        [Fact]
        public async Task CreateGroup_ReturnsOk_WhenGroupIsCreatedSuccessfully()
        {
            // Arrange
            CreateGroupVM createGroupVM = new CreateGroupVM();
            _mockAuthUtils.Setup(x => x.ExtractUserIdFromJWT(It.IsAny<ClaimsPrincipal>())).Returns(1);
            _mockChatRoomService.Setup(x => x.CreateGroupWithSelectedUsers(It.IsAny<CreateGroupVM>()))
                                .ReturnsAsync(new List<ChatlistVM>());

            // Act
            IActionResult result = await _controller.CreateGroup(createGroupVM);

            // Assert
            OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
            _mockChatRoomService.Verify(x => x.CreateGroupWithSelectedUsers(It.IsAny<CreateGroupVM>()), Times.Once);
        }


        [Fact]
        public async Task AddMembersToGroup_ReturnsOk_WhenAddMemberIsSuccessfully()
        {
            // Arrange
            AddMemberVM addMemberVM = new AddMemberVM
            {
                ChatRoomId = 1, 
                SelectedUsers = new List<int> { 2, 3, 4 } 
            };
            _mockAuthUtils.Setup(x => x.ExtractUserIdFromJWT(It.IsAny<ClaimsPrincipal>())).Returns(1);
            _mockChatRoomService.Setup(x => x.AddMembersToGroup(It.IsAny<AddMemberVM>()))
                                .ReturnsAsync(new List<ChatlistVM>());

            // Act
            ActionResult<int> result = await _controller.AddMembersToGroup(addMemberVM);

            // Assert
            ActionResult<int> okResult = Assert.IsType<ActionResult<int>>(result);
            Assert.NotNull(okResult);
            _mockChatRoomService.Verify(x => x.AddMembersToGroup(It.IsAny<AddMemberVM>()), Times.Once);
        }

        [Fact]
        public async Task RemoveUserFromGroup_ReturnsOk_WhenGroupIsCreatedSuccessfully()
        {
            // Arrange
            int chatRoomId = 1;
            int removedUserId = 2;
            int InitiatedBy = 1;

            _mockAuthUtils.Setup(x => x.ExtractUserIdFromJWT(It.IsAny<ClaimsPrincipal>())).Returns(1);
            _mockChatRoomService.Setup(x => x.RemoveUserFromGroup(It.IsAny<int>(), It.IsAny<int>()))
                                .ReturnsAsync(1);

            // Act
            ActionResult<int> result = await _controller.RemoveUserFromGroup(chatRoomId,removedUserId,InitiatedBy);

            // Assert
            ActionResult<int> okResult = Assert.IsType<ActionResult<int>>(result);
            Assert.NotNull(okResult);
            _mockChatRoomService.Verify(x => x.RemoveUserFromGroup(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task RemoveUserFromGroup_ReturnsBadRequest_WhenInitiatedByIsNotAuthorized()
        {
            // Arrange
            int chatRoomId = 1;
            int removedUserId = 1;
            int InitiatedBy = 2;

            _mockAuthUtils.Setup(x => x.ExtractUserIdFromJWT(It.IsAny<ClaimsPrincipal>())).Returns(1);

            // Act
            ActionResult<int> result = await _controller.RemoveUserFromGroup(chatRoomId, removedUserId, InitiatedBy);

            // Assert
            ActionResult<int> okResult = Assert.IsType<ActionResult<int>>(result);
            Assert.NotNull(okResult);
        }


        [Fact]
        public async Task QuitGroup_ReturnsOk_WhenIsQuitGroupSuccessfully()
        {
            // Arrange
            int chatRoomId = 1;

            _mockAuthUtils.Setup(x => x.ExtractUserIdFromJWT(It.IsAny<ClaimsPrincipal>())).Returns(1);
            _mockChatRoomService.Setup(x => x.QuitGroup(It.IsAny<int>(), It.IsAny<int>()))
                                .ReturnsAsync(1);

            // Act
            ActionResult<int> result = await _controller.QuitGroup(chatRoomId);

            // Assert
            ActionResult<int> okResult = Assert.IsType<ActionResult<int>>(result);
            Assert.NotNull(okResult);
            _mockChatRoomService.Verify(x => x.QuitGroup(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
        }


        [Fact]
        public async Task UpdateGroupName_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("error", "some error");

            // Act
            IActionResult result = await _controller.UpdateGroupName(new UpdateGroupName());

            // Assert
            BadRequestObjectResult badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid request data", badRequest.Value);
        }

        [Fact]
        public async Task UpdateGroupName_ReturnsOk_WhenGroupNameIsUpdateSuccessfully()
        {
            // Arrange
            UpdateGroupName groupName = new UpdateGroupName();
            _mockAuthUtils.Setup(x => x.ExtractUserIdFromJWT(It.IsAny<ClaimsPrincipal>())).Returns(1);
            _mockChatRoomService.Setup(x => x.UpdateGroupName(It.IsAny<int>(), It.IsAny<string>()))
                                .ReturnsAsync(1);

            // Act
            var result = await _controller.UpdateGroupName(groupName);

            // Assert
            OkResult okResult = Assert.IsType<OkResult>(result);
            Assert.NotNull(okResult);
            _mockChatRoomService.Verify(x => x.UpdateGroupName(It.IsAny<int>(), It.IsAny<string>()), Times.Once);
        }


        [Fact]
        public async Task UpdateGroupName_ReturnNoFound_WhenGroupNameIsFailUpdateSuccessfully()
        {
            // Arrange
            UpdateGroupName groupName = new UpdateGroupName();
            _mockAuthUtils.Setup(x => x.ExtractUserIdFromJWT(It.IsAny<ClaimsPrincipal>())).Returns(1);
            _mockChatRoomService.Setup(x => x.UpdateGroupName(It.IsAny<int>(), It.IsAny<string>()))
                                .ReturnsAsync(0);

            // Act
            var result = await _controller.UpdateGroupName(groupName);

            // Assert
            NotFoundObjectResult notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Group invalid or not found", notFound.Value);
            _mockChatRoomService.Verify(x => x.UpdateGroupName(It.IsAny<int>(), It.IsAny<string>()), Times.Once);
        }

    }
}
