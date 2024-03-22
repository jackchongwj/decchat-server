using ChatroomB_Backend.DTO;
using ChatroomB_Backend.Hubs;
using ChatroomB_Backend.Models;
using ChatroomB_Backend.Repository;
using ChatroomB_Backend.Service;
using Microsoft.AspNetCore.SignalR;
using Moq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatroomB_Backend_Test.ServiceTest
{
    public class UserServiceTest
    {
        private readonly Mock<IUserRepo> _mockUserRepo;
        private readonly Mock<IHubContext<ChatHub>> _mockHubContext;
        private readonly Mock<IBlobService> _mockBlockService;
        private readonly Mock<IRedisServcie> _mockRedisService;
        private readonly Mock<IHubClients> _mockClients;
        private readonly Mock<IClientProxy> _mockClientProxy;
        private readonly UsersServices _service;

        public UserServiceTest()
        {
            _mockUserRepo = new Mock<IUserRepo>();
            _mockHubContext = new Mock<IHubContext<ChatHub>>();
            _mockBlockService = new Mock<IBlobService>();
            _mockRedisService = new Mock<IRedisServcie>();
            _service = new UsersServices(_mockUserRepo.Object,  _mockBlockService.Object, _mockRedisService.Object, _mockHubContext.Object);

            // Mock Clients property
            _mockClients = new Mock<IHubClients>();
            _mockHubContext.Setup(h => h.Clients).Returns(_mockClients.Object);
            // Mock Group method to return a mock IClientProxy
            _mockClientProxy = new Mock<IClientProxy>();
            _mockClients.Setup(clients => clients.Group(It.IsAny<string>())).Returns(_mockClientProxy.Object);
        }


        [Fact]
        public async Task GetByName_CallsExpectedMethods()
        {
            string profileName = "abcd";
            int userId =  1;
            List<UserSearchDetails> fackUserSearchDetails = new List<UserSearchDetails> 
            {
                new UserSearchDetails{
                    UserId = 2,
                    UserName = "JaneDoe456",
                    ProfileName = "Jane Doe",
                    Password = "AnotherSecurePassword456!",
                    ProfilePicture = "https://example.com/path/to/profile/picture2.jpg",
                    Status = 1,
                    IsDeleted = false
                }
            };


            _mockUserRepo.Setup(repo => repo.GetByName(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(fackUserSearchDetails);


            IEnumerable<UserSearchDetails> result = await _service.GetByName(profileName, userId);

            _mockUserRepo.Verify(repo => repo.GetByName(It.IsAny<string>(), It.IsAny<int>()), Times.Once);
        }


        [Fact]
        public async Task GetFriendRequest_CallsExpectedMethods()
        {
            int userId = 1;
            List<Users> fackUser = new List<Users>
            {
                new Users{
                    UserId = 2,
                    UserName = "JohnDoe456",
                    ProfileName = "John Doe",
                    HashedPassword = "another_hashed_password",
                    Salt = "another_random_salt_value",
                    ProfilePicture = "https://example.com/path/to/profile/picture2.jpg",
                    IsDeleted = false
                }
            };


            _mockUserRepo.Setup(repo => repo.GetFriendRequest(It.IsAny<int>())).ReturnsAsync(fackUser);


            IEnumerable<Users> result = await _service.GetFriendRequest(userId);

            _mockUserRepo.Verify(repo => repo.GetFriendRequest(It.IsAny<int>()), Times.Once);
        }


        [Fact]
        public async Task GetUserById_CallsExpectedMethods()
        {
            int userId = 1;
            Users fackUser = new Users
            {
                UserId = 2,
                UserName = "JohnDoe456",
                ProfileName = "John Doe",
                HashedPassword = "another_hashed_password",
                Salt = "another_random_salt_value",
                ProfilePicture = "https://example.com/path/to/profile/picture2.jpg",
                IsDeleted = false
            };



            _mockUserRepo.Setup(repo => repo.GetUserById(It.IsAny<int>())).ReturnsAsync(fackUser);


            Users result = await _service.GetUserById(userId);

            _mockUserRepo.Verify(repo => repo.GetUserById(It.IsAny<int>()), Times.Once);
        }


        [Fact]
        public async Task UpdateProfileName_CallsExpectedMethods()
        {
            int userId = 1;
            string newProfileName = "abcd";
            List<ChatlistVM> expectedChatList = new List<ChatlistVM>
            {
                new ChatlistVM
                {
                    ChatRoomId = 1,
                    UserChatRoomId = 1,
                    UserId = 1,
                    ProfileName = "User One",
                    ProfilePicture = "user1pic.jpg",
                    ChatRoomName = "Room One",
                    RoomType = true,
                    SelectedUsers = new DataTable(),
                    InitiatedBy = 1,
                    InitiatorProfileName = "User One",
                    IsOnline = true
                },
            };

            _mockUserRepo.Setup(repo => repo.UpdateProfileName(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(1);
            _mockUserRepo.Setup(repo => repo.GetChatListByUserId(It.IsAny<int>())).ReturnsAsync(expectedChatList);

            int result = await _service.UpdateProfileName(userId, newProfileName);

            _mockUserRepo.Verify(repo => repo.UpdateProfileName(It.IsAny<int>(), It.IsAny<string>()), Times.Once);
            _mockUserRepo.Verify(repo => repo.GetChatListByUserId(It.IsAny<int>()), Times.Once);
            foreach (var chatlist in expectedChatList)
            {
                _mockClients.Verify(clients => clients.Group(It.Is<string>(s => s == $"User{chatlist.UserId}")), Times.Once);
            }
            _mockClientProxy.Verify(clientProxy => clientProxy.SendCoreAsync("ReceiveUserProfileUpdate", It.IsAny<object[]>(), It.IsAny<CancellationToken>()), Times.Once);
        }

    }
}
