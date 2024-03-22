using ChatroomB_Backend.Controllers;
using ChatroomB_Backend.DTO;
using ChatroomB_Backend.Hubs;
using ChatroomB_Backend.Models;
using ChatroomB_Backend.Repository;
using ChatroomB_Backend.Service;
using ChatroomB_Backend.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatroomB_Backend_Test.ServiceTest
{

    public class FriendServiceTest
    {
        private readonly Mock<IFriendRepo> _mockFriendRepo;
        private readonly Mock<IHubContext<ChatHub>> _mockHubContext;
        private readonly Mock<IRedisServcie> _mockRedisService;
        private readonly Mock<IHubClients> _mockClients;
        private readonly Mock<IClientProxy> _mockClientProxy;
        private readonly FriendsServices _service;


        public FriendServiceTest()
        {
            _mockFriendRepo = new Mock<IFriendRepo>();
            _mockHubContext = new Mock<IHubContext<ChatHub>>();
            _mockRedisService = new Mock<IRedisServcie>(); 
            _service = new FriendsServices(_mockFriendRepo.Object, _mockHubContext.Object, _mockRedisService.Object);

            // Mock Clients property
            _mockClients = new Mock<IHubClients>();
            _mockHubContext.Setup(h => h.Clients).Returns(_mockClients.Object);
            // Mock Group method to return a mock IClientProxy
            _mockClientProxy = new Mock<IClientProxy>();
            _mockClients.Setup(clients => clients.Group(It.IsAny<string>())).Returns(_mockClientProxy.Object);

        }

        [Fact]
        public async Task AddFriends_CallsExpectedMethods()
        {
            Friends fakeFriends = new Friends { SenderId = 1, ReceiverId = 2 };

            List<Users> user = new List<Users>
            {
              new Users
              {
                UserId = 1,
                UserName = "testuser",
                ProfileName = "Test User 1",
                HashedPassword = "hashedPassword1",
                Salt = "salt1",
                ProfilePicture = "profilePicturePath1.jpg",
                IsDeleted = false
              }
            };

            _mockFriendRepo.Setup(repo => repo.AddFriends(It.IsAny<Friends>())).ReturnsAsync(user);

            IEnumerable<Users> result = await _service.AddFriends(fakeFriends);

            _mockFriendRepo.Verify(repo => repo.AddFriends(It.IsAny<Friends>()), Times.Once);
            _mockClients.Verify(clients => clients.Group(It.Is<string>(s => s ==$"User{fakeFriends.SenderId}")), Times.Once);
            _mockClients.Verify(clients => clients.Group(It.Is<string>(s => s ==$"User{fakeFriends.ReceiverId}")), Times.Exactly(2));
            _mockClientProxy.Verify(clientProxy => clientProxy.SendCoreAsync("UpdateSearchResults",It.IsAny<object[]>(),It.IsAny<CancellationToken>()),Times.Exactly(2));
            _mockClientProxy.Verify(clientProxy => clientProxy.SendCoreAsync("UpdateFriendRequest", It.IsAny<object[]>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateFriendRequest_CallsExpectedMethods_WhenAccepted()
        {
            FriendRequest fakeFriends = new FriendRequest { SenderId = 1, ReceiverId = 2, Status = 2};


            _mockFriendRepo.Setup(repo => repo.UpdateFriendRequest(It.IsAny<FriendRequest>())).ReturnsAsync(1);

            int result = await _service.UpdateFriendRequest(fakeFriends);

            _mockFriendRepo.Verify(repo => repo.UpdateFriendRequest(It.IsAny<FriendRequest>()), Times.Once);
            _mockClients.Verify(clients => clients.Group(It.Is<string>(s => s ==$"User{fakeFriends.SenderId}")), Times.Once);
            _mockClients.Verify(clients => clients.Group(It.Is<string>(s => s ==$"User{fakeFriends.ReceiverId}")), Times.Once);
            _mockClientProxy.Verify(clientProxy => clientProxy.SendCoreAsync("UpdateSearchResultsAfterAccept", It.IsAny<object[]>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        }

        [Fact]
        public async Task UpdateFriendRequest_CallsExpectedMethods_WhenRejected()
        {
            FriendRequest fakeFriends = new FriendRequest { SenderId = 1, ReceiverId = 2, Status = 3 };


            _mockFriendRepo.Setup(repo => repo.UpdateFriendRequest(It.IsAny<FriendRequest>())).ReturnsAsync(1);

            int result = await _service.UpdateFriendRequest(fakeFriends);

            _mockFriendRepo.Verify(repo => repo.UpdateFriendRequest(It.IsAny<FriendRequest>()), Times.Once);
            _mockClients.Verify(clients => clients.Group(It.Is<string>(s => s ==$"User{fakeFriends.SenderId}")), Times.Once);
            _mockClients.Verify(clients => clients.Group(It.Is<string>(s => s ==$"User{fakeFriends.ReceiverId}")), Times.Once);
            _mockClientProxy.Verify(clientProxy => clientProxy.SendCoreAsync("UpdateSearchResultsAfterReject", It.IsAny<object[]>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        }


        [Fact]
        public async Task DeleteFriendRequest_CallsExpectedMethods_WhenDeleted()
        {
            int chatroomId = 1;
            int userId1 = 1;
            int userId2 = 2;

            _mockFriendRepo.Setup(repo => repo.DeleteFriendRequest(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(1);

            var result = await _service.DeleteFriendRequest(chatroomId,userId1,userId2);

            _mockFriendRepo.Verify(repo => repo.DeleteFriendRequest(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            _mockClients.Verify(clients => clients.Group(It.Is<string>(s => s ==$"User{userId1}")), Times.Once);
            _mockClients.Verify(clients => clients.Group(It.Is<string>(s => s ==$"User{userId2}")), Times.Once);
            _mockClientProxy.Verify(clientProxy => clientProxy.SendCoreAsync("DeleteFriend", It.IsAny<object[]>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        }

        [Fact]
        public async Task CheckFriendExist_CallsExpectedMethods()
        {
            Friends fakeFriends = new Friends { SenderId = 1, ReceiverId = 2 };

            _mockFriendRepo.Setup(repo => repo.CheckFriendExist(It.IsAny<Friends>())).ReturnsAsync(1);

            var result = await _service.CheckFriendExist(fakeFriends);

            _mockFriendRepo.Verify(repo => repo.CheckFriendExist(It.IsAny<Friends>()), Times.Once);
        }
    }
}
