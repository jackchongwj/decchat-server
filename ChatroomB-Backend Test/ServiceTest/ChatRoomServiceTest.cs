using ChatroomB_Backend.DTO;
using ChatroomB_Backend.Hubs;
using ChatroomB_Backend.Models;
using ChatroomB_Backend.Repository;
using ChatroomB_Backend.Service;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatroomB_Backend_Test.ServiceTest
{
    public class ChatRoomServiceTest
    {
        private readonly Mock<IChatRoomRepo> _mockChatRoomRepo;
        private readonly Mock<IHubContext<ChatHub>> _mockHubContext;
        private readonly Mock<IBlobService> _mockBlobService;
        private readonly Mock<IRedisServcie> _mockRedisService;
        private readonly Mock<IHubClients> _mockClients;
        private readonly Mock<IClientProxy> _mockClientProxy;
        private readonly ChatRoomServices _service;

        public ChatRoomServiceTest()
        {
            _mockChatRoomRepo = new Mock<IChatRoomRepo>();
            _mockHubContext = new Mock<IHubContext<ChatHub>>();
            _mockBlobService = new Mock<IBlobService>();
            _mockRedisService = new Mock<IRedisServcie>();
            _service = new ChatRoomServices(_mockChatRoomRepo.Object, _mockHubContext.Object, _mockBlobService.Object, _mockRedisService.Object);

            // Mock Clients property
            _mockClients = new Mock<IHubClients>();
            _mockHubContext.Setup(h => h.Clients).Returns(_mockClients.Object);
            // Mock Group method to return a mock IClientProxy
            _mockClientProxy = new Mock<IClientProxy>();
            _mockClients.Setup(clients => clients.Group(It.IsAny<string>())).Returns(_mockClientProxy.Object);
        }

        [Fact]
        public async Task AddChatRoom_ConnectionIdIsNotValid_ReceiverIdEqualUserID_IsNotOnline()
        {
            FriendRequest fakeFriends = new FriendRequest { SenderId = 1, ReceiverId = 2};
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
                new ChatlistVM
                {
                    ChatRoomId = 2,
                    UserChatRoomId = 2,
                    UserId = 2,
                    ProfileName = "User Two",
                    ProfilePicture = "user2pic.jpg",
                    ChatRoomName = "Room Two",
                    RoomType = false, 
                    SelectedUsers = new DataTable(), 
                    InitiatedBy = 2,
                    InitiatorProfileName = "User Two",
                    IsOnline = false
                },
            };
            List<string> onlineUserIds = new List<string> { "" };

            _mockChatRoomRepo.Setup(repo => repo.AddChatRoom(It.IsAny<FriendRequest>())).ReturnsAsync(expectedChatList);
            _mockRedisService.Setup(repo => repo.GetAllUserIdsFromRedisSet()).ReturnsAsync(onlineUserIds);
            _mockRedisService.Setup(repo => repo.SelectUserIdFromRedis(It.IsAny<int>())).ReturnsAsync("Hash entry not found or empty.");
            _mockHubContext.Setup(repo => repo.Groups.AddToGroupAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);


            IEnumerable<ChatlistVM> result = await _service.AddChatRoom(fakeFriends);

            _mockChatRoomRepo.Verify(repo => repo.AddChatRoom(It.IsAny<FriendRequest>()), Times.Once);
            _mockRedisService.Verify(repo => repo.GetAllUserIdsFromRedisSet(), Times.Once);
            _mockRedisService.Verify(repo => repo.SelectUserIdFromRedis(It.IsAny<int>()), Times.Exactly(2));
            _mockHubContext.Verify(repo => repo.Groups.AddToGroupAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockClients.Verify(clients => clients.Group(It.Is<string>(s => s ==$"User{fakeFriends.ReceiverId}")), Times.Once);
            _mockClientProxy.Verify(clientProxy => clientProxy.SendCoreAsync("UpdatePrivateChatlist", It.IsAny<object[]>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AddChatRoom_ConnectionIdIsNotValid_ReceiverIdNotEqualUserID_IsOnline()
        {
            FriendRequest fakeFriends = new FriendRequest { SenderId = 1, ReceiverId = 3 };
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
                new ChatlistVM
                {
                    ChatRoomId = 2,
                    UserChatRoomId = 2,
                    UserId = 2,
                    ProfileName = "User Two",
                    ProfilePicture = "user2pic.jpg",
                    ChatRoomName = "Room Two",
                    RoomType = false,
                    SelectedUsers = new DataTable(),
                    InitiatedBy = 2,
                    InitiatorProfileName = "User Two",
                    IsOnline = false
                },
            };
            List<string> onlineUserIds = new List<string> { "1" };

            _mockChatRoomRepo.Setup(repo => repo.AddChatRoom(It.IsAny<FriendRequest>())).ReturnsAsync(expectedChatList);
            _mockRedisService.Setup(repo => repo.GetAllUserIdsFromRedisSet()).ReturnsAsync(onlineUserIds);
            _mockRedisService.Setup(repo => repo.SelectUserIdFromRedis(It.IsAny<int>())).ReturnsAsync("Hash entry not found or empty.");
            _mockHubContext.Setup(repo => repo.Groups.AddToGroupAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            IEnumerable<ChatlistVM> result = await _service.AddChatRoom(fakeFriends);

            _mockChatRoomRepo.Verify(repo => repo.AddChatRoom(It.IsAny<FriendRequest>()), Times.Once);
            _mockRedisService.Verify(repo => repo.GetAllUserIdsFromRedisSet(), Times.Once);
            _mockRedisService.Verify(repo => repo.SelectUserIdFromRedis(It.IsAny<int>()), Times.Exactly(2));
            _mockHubContext.Verify(repo => repo.Groups.AddToGroupAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockClients.Verify(clients => clients.Group(It.Is<string>(s => s ==$"User{fakeFriends.ReceiverId}")), Times.Once);
            _mockClients.Verify(clients => clients.Group(It.Is<string>(s => s =="1")), Times.Exactly(2));
            _mockClientProxy.Verify(clientProxy => clientProxy.SendCoreAsync("UpdateUserOnlineStatus", It.IsAny<object[]>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
            _mockClientProxy.Verify(clientProxy => clientProxy.SendCoreAsync("UpdatePrivateChatlist", It.IsAny<object[]>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AddChatRoom_ConnectionIdIsValid_ReceiverIdEqualUserID_IsOnline()
        {
            FriendRequest fakeFriends = new FriendRequest { SenderId = 2, ReceiverId = 1 };
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
                new ChatlistVM
                {
                    ChatRoomId = 2,
                    UserChatRoomId = 2,
                    UserId = 2,
                    ProfileName = "User Two",
                    ProfilePicture = "user2pic.jpg",
                    ChatRoomName = "Room Two",
                    RoomType = false,
                    SelectedUsers = new DataTable(),
                    InitiatedBy = 2,
                    InitiatorProfileName = "User Two",
                    IsOnline = false
                },
            };
            List<string> onlineUserIds = new List<string> { "1" };

            _mockChatRoomRepo.Setup(repo => repo.AddChatRoom(It.IsAny<FriendRequest>())).ReturnsAsync(expectedChatList);
            _mockRedisService.Setup(repo => repo.GetAllUserIdsFromRedisSet()).ReturnsAsync(onlineUserIds);
            _mockRedisService.Setup(repo => repo.SelectUserIdFromRedis(It.IsAny<int>())).ReturnsAsync("abcd");
            _mockHubContext.Setup(repo => repo.Groups.AddToGroupAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            IEnumerable<ChatlistVM> result = await _service.AddChatRoom(fakeFriends);

            _mockChatRoomRepo.Verify(repo => repo.AddChatRoom(It.IsAny<FriendRequest>()), Times.Once);
            _mockRedisService.Verify(repo => repo.GetAllUserIdsFromRedisSet(), Times.Once);
            _mockRedisService.Verify(repo => repo.SelectUserIdFromRedis(It.IsAny<int>()), Times.Exactly(2));
            _mockHubContext.Verify(repo => repo.Groups.AddToGroupAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
            _mockClients.Verify(clients => clients.Group(It.Is<string>(s => s == $"User{fakeFriends.ReceiverId}")), Times.Once);
            _mockClients.Verify(clients => clients.Group(It.Is<string>(s => s == $"User{fakeFriends.SenderId}")), Times.Once);
            _mockClients.Verify(clients => clients.Group(It.Is<string>(s => s ==$"1")), Times.Once);
            _mockClientProxy.Verify(clientProxy => clientProxy.SendCoreAsync("UpdateUserOnlineStatus", It.IsAny<object[]>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockClientProxy.Verify(clientProxy => clientProxy.SendCoreAsync("UpdatePrivateChatlist", It.IsAny<object[]>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        }

        [Fact]
        public async Task AddChatRoom_ConnectionIdIsValid_ReceiverIdNotEqualUserID_IsNotOnline()
        {
            FriendRequest fakeFriends = new FriendRequest { SenderId = 1, ReceiverId = 2 };
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
                new ChatlistVM
                {
                    ChatRoomId = 2,
                    UserChatRoomId = 2,
                    UserId = 2,
                    ProfileName = "User Two",
                    ProfilePicture = "user2pic.jpg",
                    ChatRoomName = "Room Two",
                    RoomType = false,
                    SelectedUsers = new DataTable(),
                    InitiatedBy = 2,
                    InitiatorProfileName = "User Two",
                    IsOnline = false
                },
            };
            List<string> onlineUserIds = new List<string> { "1" };

            _mockChatRoomRepo.Setup(repo => repo.AddChatRoom(It.IsAny<FriendRequest>())).ReturnsAsync(expectedChatList);
            _mockRedisService.Setup(repo => repo.GetAllUserIdsFromRedisSet()).ReturnsAsync(onlineUserIds);
            _mockRedisService.Setup(repo => repo.SelectUserIdFromRedis(It.IsAny<int>())).ReturnsAsync("abcd");
            _mockHubContext.Setup(repo => repo.Groups.AddToGroupAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            IEnumerable<ChatlistVM> result = await _service.AddChatRoom(fakeFriends);

            _mockChatRoomRepo.Verify(repo => repo.AddChatRoom(It.IsAny<FriendRequest>()), Times.Once);
            _mockRedisService.Verify(repo => repo.GetAllUserIdsFromRedisSet(), Times.Once);
            _mockRedisService.Verify(repo => repo.SelectUserIdFromRedis(It.IsAny<int>()), Times.Exactly(2));
            _mockHubContext.Verify(repo => repo.Groups.AddToGroupAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
            _mockClients.Verify(clients => clients.Group(It.Is<string>(s => s == $"User{fakeFriends.ReceiverId}")), Times.Once); 
            _mockClients.Verify(clients => clients.Group(It.Is<string>(s => s == $"User{fakeFriends.SenderId}")), Times.Once);
            _mockClientProxy.Verify(clientProxy => clientProxy.SendCoreAsync("UpdatePrivateChatlist", It.IsAny<object[]>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        }


        [Fact]
        public async Task CreateGroupWithSelectedUsers_CallsExpectedMethods()
        {
            CreateGroupVM createGroupVM = new CreateGroupVM
            {
                RoomName = "Test Group",
                ChatRoomId = 1,
                SelectedUsers = new List<int> { 2, 3, 4 },
                InitiatedBy = 1,
                UserId = 1
            };

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
                new ChatlistVM
                {
                    ChatRoomId = 2,
                    UserChatRoomId = 2,
                    UserId = 2,
                    ProfileName = "User Two",
                    ProfilePicture = "user2pic.jpg",
                    ChatRoomName = "Room Two",
                    RoomType = false,
                    SelectedUsers = new DataTable(),
                    InitiatedBy = 2,
                    InitiatorProfileName = "User Two",
                    IsOnline = false
                },
            };

            _mockChatRoomRepo.Setup(repo => repo.CreateGroup(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<DataTable>())).ReturnsAsync(expectedChatList);
            _mockRedisService.Setup(repo => repo.SelectUserIdFromRedis(It.IsAny<int>())).ReturnsAsync("abcd");
            _mockHubContext.Setup(repo => repo.Groups.AddToGroupAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            IEnumerable<ChatlistVM> result = await _service.CreateGroupWithSelectedUsers(createGroupVM);

            _mockChatRoomRepo.Verify(repo => repo.CreateGroup(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<DataTable>()), Times.Once);
            _mockRedisService.Verify(repo => repo.SelectUserIdFromRedis(It.IsAny<int>()), Times.Exactly(4));
            _mockHubContext.Verify(repo => repo.Groups.AddToGroupAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Exactly(4));
            _mockClients.Verify(clients => clients.Group(It.Is<string>(s => s == "1")), Times.Once);
            _mockClientProxy.Verify(clientProxy => clientProxy.SendCoreAsync("NewGroupCreated", It.IsAny<object[]>(), It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        public async Task AddMembersToGroup_CallsExpectedMethods()
        {
            AddMemberVM addMemberVM = new AddMemberVM
            {
                ChatRoomId = 1,
                SelectedUsers = new List<int> { 1, 2},
            };

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
                new ChatlistVM
                {
                    ChatRoomId = 2,
                    UserChatRoomId = 2,
                    UserId = 2,
                    ProfileName = "User Two",
                    ProfilePicture = "user2pic.jpg",
                    ChatRoomName = "Room Two",
                    RoomType = false,
                    SelectedUsers = new DataTable(),
                    InitiatedBy = 2,
                    InitiatorProfileName = "User Two",
                    IsOnline = false
                },
            };

            _mockChatRoomRepo.Setup(repo => repo.AddMembersToGroup(It.IsAny<int>(), It.IsAny<DataTable>())).ReturnsAsync(expectedChatList);
            _mockChatRoomRepo.Setup(repo => repo.GetGroupInfoByChatroomId(It.IsAny<int>(), It.IsAny<DataTable>())).ReturnsAsync(expectedChatList);

            _mockRedisService.Setup(repo => repo.SelectUserIdFromRedis(It.IsAny<int>())).ReturnsAsync("abcd");
            _mockHubContext.Setup(repo => repo.Groups.AddToGroupAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            IEnumerable<ChatlistVM> result = await _service.AddMembersToGroup(addMemberVM);

            _mockChatRoomRepo.Verify(repo => repo.AddMembersToGroup(It.IsAny<int>(), It.IsAny<DataTable>()), Times.Once);
            _mockChatRoomRepo.Verify(repo => repo.GetGroupInfoByChatroomId(It.IsAny<int>(), It.IsAny<DataTable>()), Times.Once);
            _mockRedisService.Verify(repo => repo.SelectUserIdFromRedis(It.IsAny<int>()), Times.Exactly(2));
            _mockHubContext.Verify(repo => repo.Groups.AddToGroupAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
            _mockClients.Verify(clients => clients.Group(It.Is<string>(s => s == "1")), Times.Once);
            _mockClientProxy.Verify(clientProxy => clientProxy.SendCoreAsync("UserAdded", It.IsAny<object[]>(), It.IsAny<CancellationToken>()), Times.Once);
            foreach (var userId in addMemberVM.SelectedUsers)
            {
                _mockClients.Verify(clients => clients.Group(It.Is<string>(s => s == $"User{userId}")), Times.Once);
            }
            _mockClientProxy.Verify(clientProxy => clientProxy.SendCoreAsync("NewGroupCreated", It.IsAny<object[]>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        }


        [Fact]
        public async Task RemoveUserFromGroup_CallsExpectedMethods()
        {
            int chatRoomId = 1;
            int removedUserId = 1;

            _mockChatRoomRepo.Setup(repo => repo.RemoveUserFromGroup(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(1);
            _mockRedisService.Setup(repo => repo.SelectUserIdFromRedis(It.IsAny<int>())).ReturnsAsync("abcd");
            _mockHubContext.Setup(repo => repo.Groups.RemoveFromGroupAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            int result = await _service.RemoveUserFromGroup(chatRoomId,removedUserId);

            _mockChatRoomRepo.Verify(repo => repo.RemoveUserFromGroup(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            _mockRedisService.Verify(repo => repo.SelectUserIdFromRedis(It.IsAny<int>()), Times.Once);
            _mockHubContext.Verify(repo => repo.Groups.RemoveFromGroupAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockClients.Verify(clients => clients.Group(It.Is<string>(s => s == "1")), Times.Once);
            _mockClients.Verify(clients => clients.Group(It.Is<string>(s => s == $"User{removedUserId}")), Times.Once);
            _mockClientProxy.Verify(clientProxy => clientProxy.SendCoreAsync("UserRemoved", It.IsAny<object[]>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        }

        [Fact]
        public async Task RemoveUserFromGroup_CallsExpectedMethods_WhenConnectionIdIsEmpty()
        {
            int chatRoomId = 1;
            int removedUserId = 1;

            _mockChatRoomRepo.Setup(repo => repo.RemoveUserFromGroup(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(1);
            _mockRedisService.Setup(repo => repo.SelectUserIdFromRedis(It.IsAny<int>())).ReturnsAsync("Hash entry not found or empty.");

            int result = await _service.RemoveUserFromGroup(chatRoomId, removedUserId);

            _mockChatRoomRepo.Verify(repo => repo.RemoveUserFromGroup(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            _mockRedisService.Verify(repo => repo.SelectUserIdFromRedis(It.IsAny<int>()), Times.Once);
            _mockClients.Verify(clients => clients.Group(It.Is<string>(s => s == "1")), Times.Once);
            _mockClients.Verify(clients => clients.Group(It.Is<string>(s => s == $"User{removedUserId}")), Times.Once);
            _mockClientProxy.Verify(clientProxy => clientProxy.SendCoreAsync("UserRemoved", It.IsAny<object[]>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        }


        [Fact]
        public async Task UpdateGroupName_CallsExpectedMethods()
        {
            int chatRoomId = 1;
            string newGroupName = "abcd";

            _mockChatRoomRepo.Setup(repo => repo.UpdateGroupName(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(1);

            int result = await _service.UpdateGroupName(chatRoomId, newGroupName);

            _mockChatRoomRepo.Verify(repo => repo.UpdateGroupName(It.IsAny<int>(), It.IsAny<string>()), Times.Once);
            _mockClients.Verify(clients => clients.Group(It.Is<string>(s => s == "1")), Times.Once);
            _mockClientProxy.Verify(clientProxy => clientProxy.SendCoreAsync("ReceiveGroupProfileUpdate", It.IsAny<object[]>(), It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact] 
        public async Task UpdateGroupPicture_CallsExpectedMethods()
        {
            int chatRoomId = 1;
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string imagePath = Path.Combine(desktopPath, "testImage.jpg");
            byte[] fileBytes = File.ReadAllBytes(imagePath);
            string fileName = "image.jpg";
            string expectedBlobUri = "https://example.com/image.jpg";
 

            
            _mockBlobService.Setup(repo => repo.UploadImageFiles(It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(expectedBlobUri);
            _mockChatRoomRepo.Setup(repo => repo.UpdateGroupPicture(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(1);

            int result = await _service.UpdateGroupPicture(chatRoomId,fileBytes, fileName);

            _mockBlobService.Verify(x => x.UploadImageFiles(It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once);
            _mockChatRoomRepo.Verify(repo => repo.UpdateGroupPicture(It.IsAny<int>(), It.IsAny<string>()), Times.Once);
            _mockClients.Verify(clients => clients.Group(It.Is<string>(s => s == "1")), Times.Once);
            _mockClientProxy.Verify(clientProxy => clientProxy.SendCoreAsync("ReceiveGroupProfileUpdate", It.IsAny<object[]>(), It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        public async Task RetrieveGroupMemberByChatroomId_CallsExpectedMethods()
        {
            int chatRoomId = 1;
            int userId = 1;
            List<GroupMember> groupMember = new List<GroupMember>
            {
                new GroupMember
                {
                    ChatRoomId = 1,
                    UserId = 1,
                    ProfileName = "John Doe",
                    ProfilePicture = "https://example.com/path/to/johndoe.jpg"
                }
            };

            _mockChatRoomRepo.Setup(repo => repo.RetrieveGroupMemberByChatroomId(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(groupMember);

            IEnumerable<GroupMember> result = await _service.RetrieveGroupMemberByChatroomId(chatRoomId, userId);

            _mockChatRoomRepo.Verify(x => x.RetrieveGroupMemberByChatroomId(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
        }



        [Fact]
        public async Task QuitGroup_CallsExpectedMethods_WhenConnectionIsNotNull()
        {
            int chatRoomId = 1;
            int userId = 1;

            _mockChatRoomRepo.Setup(repo => repo.QuitGroup(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(1);
            _mockRedisService.Setup(repo => repo.SelectUserIdFromRedis(It.IsAny<int>())).ReturnsAsync("abcd");
            _mockHubContext.Setup(repo => repo.Groups.RemoveFromGroupAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            int result = await _service.QuitGroup(chatRoomId, userId);

            _mockChatRoomRepo.Verify(x => x.QuitGroup(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            _mockRedisService.Verify(repo => repo.SelectUserIdFromRedis(It.IsAny<int>()), Times.Once);
            _mockHubContext.Verify(repo => repo.Groups.RemoveFromGroupAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockClients.Verify(clients => clients.Group(It.Is<string>(s => s == "1")), Times.Exactly(2));
            _mockClients.Verify(clients => clients.Group(It.Is<string>(s => s == $"User{userId}")), Times.Once);
            _mockClientProxy.Verify(clientProxy => clientProxy.SendCoreAsync("QuitGroup", It.IsAny<object[]>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
            _mockClientProxy.Verify(clientProxy => clientProxy.SendCoreAsync("UpdateInitiatedBy", It.IsAny<object[]>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task QuitGroup_CallsExpectedMethods_WhenConnectionIsNull()
        {
            int chatRoomId = 1;
            int userId = 1;

            _mockChatRoomRepo.Setup(repo => repo.QuitGroup(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(1);
            _mockRedisService.Setup(repo => repo.SelectUserIdFromRedis(It.IsAny<int>())).ReturnsAsync("Hash entry not found or empty.");
            
            int result = await _service.QuitGroup(chatRoomId, userId);

            _mockChatRoomRepo.Verify(x => x.QuitGroup(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            _mockRedisService.Verify(repo => repo.SelectUserIdFromRedis(It.IsAny<int>()), Times.Once);
            _mockClients.Verify(clients => clients.Group(It.Is<string>(s => s == "1")), Times.Exactly(2));
            _mockClients.Verify(clients => clients.Group(It.Is<string>(s => s == $"User{userId}")), Times.Once);
            _mockClientProxy.Verify(clientProxy => clientProxy.SendCoreAsync("QuitGroup", It.IsAny<object[]>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
            _mockClientProxy.Verify(clientProxy => clientProxy.SendCoreAsync("UpdateInitiatedBy", It.IsAny<object[]>(), It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        public async Task QuitGroup_CallsExpectedMethods_WhenChangeInitiator()
        {
            int chatRoomId = 1;
            int userId = 1;

            _mockChatRoomRepo.Setup(repo => repo.QuitGroup(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(0);
            _mockRedisService.Setup(repo => repo.SelectUserIdFromRedis(It.IsAny<int>())).ReturnsAsync("abcd");
            _mockHubContext.Setup(repo => repo.Groups.RemoveFromGroupAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            int result = await _service.QuitGroup(chatRoomId, userId);

            _mockChatRoomRepo.Verify(x => x.QuitGroup(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            _mockRedisService.Verify(repo => repo.SelectUserIdFromRedis(It.IsAny<int>()), Times.Once);
            _mockHubContext.Verify(repo => repo.Groups.RemoveFromGroupAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockClients.Verify(clients => clients.Group(It.Is<string>(s => s == "1")), Times.Once);
            _mockClients.Verify(clients => clients.Group(It.Is<string>(s => s == $"User{userId}")), Times.Once);
            _mockClientProxy.Verify(clientProxy => clientProxy.SendCoreAsync("QuitGroup", It.IsAny<object[]>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        }
    }
}
