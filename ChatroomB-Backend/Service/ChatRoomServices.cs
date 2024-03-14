
using Amazon.Runtime.Internal;
using Azure.Core;
using ChatroomB_Backend.DTO;
using ChatroomB_Backend.Hubs;
using ChatroomB_Backend.Models;
using ChatroomB_Backend.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text.RegularExpressions;

namespace ChatroomB_Backend.Service
{
    public class ChatRoomServices : IChatRoomService
    {
        private readonly IChatRoomRepo _repo;
        private readonly IBlobService _blobService;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IRedisServcie _RServices;


        public ChatRoomServices(IChatRoomRepo _repository, IHubContext<ChatHub> hubContext, IBlobService blobService, IRedisServcie rServices, IUserService userService)
        {
            _repo = _repository;

            _blobService = blobService;
            _hubContext = hubContext;
            _RServices = rServices;
        }

        public async Task<IEnumerable<ChatlistVM>> AddChatRoom(FriendRequest request)
        {
            IEnumerable<ChatlistVM> result = await _repo.AddChatRoom(request);

            if (!result.IsNullOrEmpty())
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

                    if (connectionIdS != "Hash entry not found or empty.")
                    {
                        await _hubContext.Groups.AddToGroupAsync(connectionIdS, groupName);
                        await _hubContext.Groups.AddToGroupAsync(connectionIdR, groupName);

                        if (request.ReceiverId == result.ElementAt(0).UserId)
                        {
                            await _hubContext.Clients.Group("User" + request.ReceiverId).SendAsync("UpdatePrivateChatlist", result.ElementAt(1));
                            await _hubContext.Clients.Group("User" + request.SenderId).SendAsync("UpdatePrivateChatlist", result.ElementAt(0));
                        }
                        else
                        {
                            await _hubContext.Clients.Group("User" + request.ReceiverId).SendAsync("UpdatePrivateChatlist", result.ElementAt(0));
                            await _hubContext.Clients.Group("User" + request.SenderId).SendAsync("UpdatePrivateChatlist", result.ElementAt(1));
                        }
                    }
                    else
                    {
                        await _hubContext.Groups.AddToGroupAsync(connectionIdR, groupName);

                        if (request.ReceiverId == result.ElementAt(1).UserId)
                        {
                            await _hubContext.Clients.Group("User" + request.ReceiverId).SendAsync("UpdatePrivateChatlist", result.ElementAt(0));
                        }
                        else
                        {
                            await _hubContext.Clients.Group("User" + request.ReceiverId).SendAsync("UpdatePrivateChatlist", result.ElementAt(1));
                        }
                    }


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

        public async Task<IEnumerable<ChatlistVM>> CreateGroupWithSelectedUsers(CreateGroupVM createGroupVM)
        {
            // Create a DataTable to store the selected user IDs
            DataTable selectedUsersTable = new DataTable();
            selectedUsersTable.Columns.Add("UserId", typeof(int));
            foreach (int userId in createGroupVM.SelectedUsers)
            {
                selectedUsersTable.Rows.Add(userId);
            }

            // Call CreateGroup method with the DataTable of selected users
            IEnumerable<ChatlistVM> chatList = await _repo.CreateGroup(createGroupVM.RoomName, createGroupVM.InitiatedBy, selectedUsersTable);
            string groupName = chatList.ToList()[0].ChatRoomId.ToString();

            foreach (int groupListUserId in createGroupVM.SelectedUsers)
            {
                // Retrieve the connection ID for the current user ID from Redis
                string userConnectionId = await _RServices.SelectUserIdFromRedis(groupListUserId);
                // Check if the connection ID is not null or empty
                if (userConnectionId != "Hash entry not found or empty.")
                {
                    await _hubContext.Groups.AddToGroupAsync(userConnectionId, groupName);
                }
            }
            // Add the admin to the group
            string adminConnectionId = await _RServices.SelectUserIdFromRedis(createGroupVM.InitiatedBy); //get admin connectionid 
            await _hubContext.Groups.AddToGroupAsync(adminConnectionId, groupName); //get admin connectionid to grp

            // Send SignalR message to the user's group using their connection ID
            await _hubContext.Clients.Group(chatList.ToList()[0].ChatRoomId.ToString()).SendAsync("NewGroupCreated", chatList);

            return chatList;
        }

        public async Task<IEnumerable<ChatlistVM>> AddMembersToGroup(AddMemberVM addMemberVM)
        {
            try
            {
                DataTable selectedUsersTable = new DataTable();
                selectedUsersTable.Columns.Add("UserId", typeof(int));
                foreach (int userId in addMemberVM.SelectedUsers)
                {
                    selectedUsersTable.Rows.Add(userId);
                }

                IEnumerable<ChatlistVM> groupProfileList = await _repo.AddMembersToGroup(addMemberVM.ChatRoomId, selectedUsersTable);
                IEnumerable<ChatlistVM> newMemberChatList = await _repo.GetGroupInfoByChatroomId(addMemberVM.ChatRoomId, selectedUsersTable);

                // Bring list to FE 
                // Send SignalR message to the user's group using their connection ID
                await _hubContext.Clients.Group(addMemberVM.ChatRoomId.ToString()).SendAsync("UserAdded", groupProfileList); //show new user to currentgrpmmb

                foreach (int groupListUserId in addMemberVM.SelectedUsers)
                {
                    //IEnumerable<ChatlistVM> chatList = await _repo.GetGroupInfoByChatroomId(addMemberVM.ChatRoomId, groupListUserId);
                    
                    // Retrieve the connection ID for the current user ID from Redis
                    string userConnectionId = await _RServices.SelectUserIdFromRedis(groupListUserId);
                    // Check if the connection ID is not null or empty
                    if (userConnectionId != "Hash entry not found or empty.")
                    {
                        await _hubContext.Groups.AddToGroupAsync(userConnectionId, addMemberVM.ChatRoomId.ToString()); //
                        await _hubContext.Clients.Group("User" + groupListUserId).SendAsync("NewGroupCreated", newMemberChatList); //kena added grp show in side bar
                    }
                }
                return groupProfileList;
            }
            catch (Exception ex)
            {
                // Log the exception
                throw new Exception("An error occurred while adding members to the group.", ex);
            }
        }

        public async Task<int> RemoveUserFromGroup(int chatRoomId, int removedUserId)
        {
            int result = await _repo.RemoveUserFromGroup(chatRoomId, removedUserId);

            if (result == 1)
            {
                string connectionId = await _RServices.SelectUserIdFromRedis(removedUserId);

                if (!connectionId.IsNullOrEmpty())
                {
                    await _hubContext.Groups.RemoveFromGroupAsync(connectionId, chatRoomId.ToString());
                    // return result;
                }

                await _hubContext.Clients.Group(chatRoomId.ToString()).SendAsync("UserRemoved", chatRoomId, removedUserId);
                await _hubContext.Clients.Group("User" + removedUserId).SendAsync("UserRemoved", chatRoomId, removedUserId);

                return result;
            }
            return result;
        }

        public async Task<int> UpdateGroupName(int chatRoomId, string newGroupName)
        {
            try
            {
                int updateResult = await _repo.UpdateGroupName(chatRoomId, newGroupName);
                if (updateResult > 0)
                {
                    await _hubContext.Clients.Groups(chatRoomId.ToString())
                        .SendAsync("ReceiveGroupProfileUpdate", new { ChatRoomId = chatRoomId, GroupName = newGroupName });
                }

                return updateResult;
            }

            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return -1;
            }
        }

        public async Task<int> UpdateGroupPicture(int chatRoomId, byte[] fileBytes, string fileName)
        {
            try
            {
                using (Image image = SixLabors.ImageSharp.Image.Load(fileBytes))
                {
                    string blobUri = await _blobService.UploadImageFiles(fileBytes, fileName, 2);

                    int updateResult = await _repo.UpdateGroupPicture(chatRoomId, blobUri);

                    if (updateResult > 0)
                    {
                        await _hubContext.Clients.Groups(chatRoomId.ToString())
                            .SendAsync("ReceiveGroupProfileUpdate", new { ChatRoomId = chatRoomId, GroupPicture = blobUri });
                    }

                    return updateResult;
                }
            }
            catch (SixLabors.ImageSharp.UnknownImageFormatException)
            {
                Console.WriteLine("Uploaded file is not a valid image.");
                return -1;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return -1;
            }
        }

        public async Task<IEnumerable<GroupMember>> RetrieveGroupMemberByChatroomId(int chatRoomId, int userId)
        {
            IEnumerable<GroupMember> result = await _repo.RetrieveGroupMemberByChatroomId(chatRoomId, userId);
            return result;
        }

        public async Task<int> QuitGroup(int chatRoomId, int userId)
        {
            int result = await _repo.QuitGroup(chatRoomId, userId);
            string connectionId = await _RServices.SelectUserIdFromRedis(userId);

            if (connectionId != "Hash entry not found or empty.")
            {
                await _hubContext.Groups.RemoveFromGroupAsync(connectionId, chatRoomId.ToString());
            }
            await _hubContext.Clients.Group(chatRoomId.ToString()).SendAsync("QuitGroup", chatRoomId, userId);
            await _hubContext.Clients.Group("User" + userId).SendAsync("QuitGroup", chatRoomId, userId);

            if (result != 0) //change initiator = 1
            {
                await _hubContext.Clients.Group(chatRoomId.ToString()).SendAsync("UpdateInitiatedBy", chatRoomId, result);
            }
            return result;
        }
    }
}
