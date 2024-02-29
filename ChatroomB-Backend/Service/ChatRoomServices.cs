
using Azure.Core;
using ChatroomB_Backend.DTO;
using ChatroomB_Backend.Hubs;
using ChatroomB_Backend.Models;
using ChatroomB_Backend.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Text.RegularExpressions;

namespace ChatroomB_Backend.Service
{
    public class ChatRoomServices : IChatRoomService
    {
        private readonly IChatRoomRepo _repo;
        private readonly IBlobService _blobService;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IRedisServcie _RServices;
        private readonly IUserService _userService;


        public ChatRoomServices(IChatRoomRepo _repository, IHubContext<ChatHub> hubContext, IBlobService blobService, IRedisServcie rServices, IUserService userService)
        {
            _repo = _repository;

            _blobService = blobService;
            _hubContext = hubContext;
            _RServices = rServices;
            _userService = userService;
     
        }

        public async Task<IEnumerable<ChatlistVM>> AddChatRoom(FriendRequest request, int userId)
        {
            IEnumerable<ChatlistVM> result = await _repo.AddChatRoom(request, userId);

            if(!result.IsNullOrEmpty()) 
            {
                try 
                {
                    // Retrieve online user IDs from Redis
                    List<string> onlineUserIds = await _RServices.GetAllUserIdsFromRedisSet();
                    // Determine if the sender is online
                    bool isSenderOnline = onlineUserIds.Contains(request.SenderId.ToString());

                    // add private list to signalR group for send message
                    string connectionIdS = await _RServices.SelectUserIdFromRedis(request.SenderId);
                    string connectionIdR = await _RServices.SelectUserIdFromRedis(request.ReceiverId);
                    string groupName = result.Select(list => list.ChatRoomId).First().ToString();

                    await _hubContext.Groups.AddToGroupAsync(connectionIdS, groupName);
                    await _hubContext.Groups.AddToGroupAsync(connectionIdR, groupName);

                    await _hubContext.Clients.Group("User"+ request.ReceiverId).SendAsync("UpdatePrivateChatlist", result.ElementAt(1));
                    await _hubContext.Clients.Group("User"+ request.SenderId).SendAsync("UpdatePrivateChatlist", result.ElementAt(0));
                    // Only send the online status update if the sender is actually online
                    if (isSenderOnline)
                    {
                        await _hubContext.Clients.Group(groupName).SendAsync("UpdateUserOnlineStatus", request.SenderId, true);
                    }
                    await _hubContext.Clients.Group(groupName).SendAsync("UpdateUserOnlineStatus", request.ReceiverId, true);
                } 
                catch (Exception ex) 
                {

                    Console.Error.WriteLine($"Error in UpdatePrivateChatlist: {ex.Message}");
                    throw;
                }
            }

            return result.ToList();
        }

        public async Task <ChatlistVM> CreateGroupWithSelectedUsers(CreateGroupVM createGroupVM)
        {
            // Create a DataTable to store the selected user IDs
            DataTable selectedUsersTable = new DataTable();
            selectedUsersTable.Columns.Add("UserId", typeof(int));
            foreach (int userId in createGroupVM.SelectedUsers)
            {
                selectedUsersTable.Rows.Add(userId);
            }
            // Call CreateGroup method with the DataTable of selected users
            var chatList = await _repo.CreateGroup(createGroupVM.RoomName, createGroupVM.InitiatedBy, selectedUsersTable);
           
            // Call AddToGroup to add users to the chat room
            var groupName = createGroupVM.ChatRoomId.ToString();
            foreach (var userId in createGroupVM.SelectedUsers)
            {
                await _hubContext.Clients.Group(groupName).SendAsync("AddToGroup", groupName, userId);
            }
            return chatList;             
        }


        public async Task <int> RemoveUserFromGroup(int chatRoomId, int userId)
        {
            int result = await _repo.RemoveUserFromGroup(chatRoomId, userId);

            if (result == 1)
            {
                string connectionId = await _RServices.SelectUserIdFromRedis(userId);

                if (!connectionId.IsNullOrEmpty())
                {
                    await _hubContext.Groups.RemoveFromGroupAsync(connectionId, chatRoomId.ToString());
                    // return result;
                }

                await _hubContext.Clients.Group(chatRoomId.ToString()).SendAsync("UserRemoved", chatRoomId, userId);
                await _hubContext.Clients.Group("User" + userId).SendAsync("UserRemoved", chatRoomId, userId);

                return result;
            }
            return result;
        }
        public async Task<int> UpdateGroupName(int chatRoomId, string newGroupName)
        {
            var updateResult = await _repo.UpdateGroupName(chatRoomId, newGroupName);
            if (updateResult > 0)
            { 
                await _hubContext.Clients.Groups(chatRoomId.ToString())
                    .SendAsync("ReceiveGroupProfileUpdate", new { ChatRoomId = chatRoomId, GroupName = newGroupName });
            }
            return updateResult;
        }

        public async Task<int> UpdateGroupPicture(int chatRoomId, byte[] fileBytes, string fileName)
        {
            try
            {
                // Upload the file to blob storage and get the URI
                string blobUri = await _blobService.UploadImageFiles(fileBytes, fileName, 2);

                // Update the user's profile picture URI in the database
                int updateResult = await _repo.UpdateGroupPicture(chatRoomId, blobUri);

                // If the profile picture was successfully updated
                if (updateResult > 0)
                {
                    await _hubContext.Clients.Groups(chatRoomId.ToString())
                        .SendAsync("ReceiveGroupProfileUpdate", new { ChatRoomId = chatRoomId, GroupPicture = blobUri });
                }

                return updateResult;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                // Return a value indicating failure, such as -1, to differentiate from successful updates
                return -1;
            }
        }


        public async Task<IEnumerable<GroupMember>> RetrieveGroupMemberByChatroomId(int chatRoomId, int userId)
        {
            return await _repo.RetrieveGroupMemberByChatroomId(chatRoomId, userId);
        }

        public async Task<int> QuitGroup(int chatRoomId, int userId)
        {
            int result = await _repo.QuitGroup(chatRoomId, userId);

            if (result == 1)
            {
                string connectionId = await _RServices.SelectUserIdFromRedis(userId);

                if (connectionId != null)
                {
                    await _hubContext.Groups.RemoveFromGroupAsync(connectionId, chatRoomId.ToString());
                    //return result;
                }
                await _hubContext.Clients.Group(chatRoomId.ToString()).SendAsync("QuitGroup", chatRoomId, userId);
                await _hubContext.Clients.Group("User" + userId).SendAsync("QuitGroup", chatRoomId, userId);
                return result;
            }
            return result;
        }

    }
}
