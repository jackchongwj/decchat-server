using ChatroomB_Backend.Controllers;
using ChatroomB_Backend.DTO;
using ChatroomB_Backend.Models;
using ChatroomB_Backend.Service;
using ChatroomB_Backend.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ChatroomB_Backend_Test.ControllerTest
{
    public class FriendControllerTest
    {
        private readonly Mock<IFriendService> _mockFriendService;
        private readonly Mock<IChatRoomService> _mockChatRoomService;
        private readonly Mock<IAuthUtils> _mockAuthUtils;
        private readonly FriendsController _controller;

        public FriendControllerTest()
        {
            _mockFriendService = new Mock<IFriendService>();
            _mockChatRoomService = new Mock<IChatRoomService>();
            _mockAuthUtils = new Mock<IAuthUtils>();
            _controller = new FriendsController(_mockFriendService.Object, _mockChatRoomService.Object, _mockAuthUtils.Object);

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
            };
        }

        [Fact]
        public async Task AddFriend_FriendNotExist_ReturnOkResult()
        {
            // Arrange
            Friends friends = new Friends { SenderId = 1, ReceiverId = 2 };
            _mockAuthUtils.Setup(x => x.ExtractUserIdFromJWT(It.IsAny<System.Security.Claims.ClaimsPrincipal>())).Returns(1);
            _mockFriendService.Setup(x => x.CheckFriendExist(friends)).ReturnsAsync(0);

            // Act
            IActionResult result = await _controller.AddFriend(friends);

            // Assert
            OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
            _mockFriendService.Verify(x => x.AddFriends(It.IsAny<Friends>()), Times.Once);
        }

        [Fact]
        public async Task AddFriend_FriendExist_ReturnBadRequestResult()
        {
            // Arrange
            Friends friends = new Friends { SenderId = 1, ReceiverId = 2 };
            _mockAuthUtils.Setup(x => x.ExtractUserIdFromJWT(It.IsAny<System.Security.Claims.ClaimsPrincipal>())).Returns(1);
            _mockFriendService.Setup(x => x.CheckFriendExist(friends)).ReturnsAsync(1);

            // Act
            IActionResult result = await _controller.AddFriend(friends);

            // Assert
            BadRequestObjectResult badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequest.Value);
        }



        [Fact]
        public async Task UpdateFriendRequest_ReturnChatlistData_WhenAddPrivateChatRoomSuccessful()
        {
            // Arrange
            FriendRequest friendsRequest = new FriendRequest { ReceiverId = 1, SenderId = 2, Status = 2 };
            List<ChatlistVM> chatList = new List<ChatlistVM> { new ChatlistVM() };

            _mockAuthUtils.Setup(x => x.ExtractUserIdFromJWT(It.IsAny<ClaimsPrincipal>())).Returns(1);
            _mockFriendService.Setup(x => x.UpdateFriendRequest(It.IsAny<FriendRequest>())).ReturnsAsync(1);
            _mockChatRoomService.Setup(x => x.AddChatRoom(It.IsAny<FriendRequest>())).ReturnsAsync(chatList);

            // Act
            ActionResult<int> result = await _controller.UpdateFriendRequest(friendsRequest);

            // Assert
            OkObjectResult okResult = Assert.IsType<OkObjectResult>(result.Result); 
            Assert.IsAssignableFrom<IEnumerable<ChatlistVM>>(okResult.Value);
            List<ChatlistVM> returnValue = Assert.IsType<List<ChatlistVM>>(okResult.Value); 
            Assert.Single(returnValue); 
            _mockFriendService.Verify(x => x.UpdateFriendRequest(It.IsAny<FriendRequest>()), Times.Once);
            _mockChatRoomService.Verify(x => x.AddChatRoom(It.IsAny<FriendRequest>()), Times.Once);
        }

        [Fact]
        public async Task UpdateFriendRequest_ReturnOkResult()
        {
            // Arrange
            FriendRequest friendsRequest = new FriendRequest { ReceiverId = 1, SenderId = 2, Status = 3 };
            List<ChatlistVM> chatList = new List<ChatlistVM> { new ChatlistVM() };

            _mockAuthUtils.Setup(x => x.ExtractUserIdFromJWT(It.IsAny<ClaimsPrincipal>())).Returns(1);
            _mockFriendService.Setup(x => x.UpdateFriendRequest(It.IsAny<FriendRequest>())).ReturnsAsync(1);
         
            // Act
            ActionResult<int> result = await _controller.UpdateFriendRequest(friendsRequest);

            // Assert
            OkObjectResult okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(1, okResult.Value);
            _mockFriendService.Verify(x => x.UpdateFriendRequest(It.IsAny<FriendRequest>()), Times.Once);
        }


        [Fact]
        public async Task DeleteFriend_ReturnsOkResult_WhenDeleteIsSuccessful()
        {
            // Arrange
            DeleteFriendRequest friendsRequest = new DeleteFriendRequest { ChatRoomId = 1, UserId2 = 2 };

            _mockAuthUtils.Setup(x => x.ExtractUserIdFromJWT(It.IsAny<ClaimsPrincipal>())).Returns(1);
            _mockFriendService.Setup(x => x.DeleteFriendRequest(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(1);

            // Act
            ActionResult<int> result = await _controller.DeleteFriend(friendsRequest);

            // Assert
            OkObjectResult okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(1, okResult.Value);
            _mockFriendService.Verify(x => x.DeleteFriendRequest(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
        }
    }
}
