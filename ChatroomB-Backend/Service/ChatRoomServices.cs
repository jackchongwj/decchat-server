
using Azure.Core;
using ChatroomB_Backend.DTO;
using ChatroomB_Backend.Hubs;
using ChatroomB_Backend.Models;
using ChatroomB_Backend.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Data;
using System.Text.RegularExpressions;

namespace ChatroomB_Backend.Service
{
    public class ChatRoomServices : IChatRoomService
    {
        private readonly IChatRoomRepo _repo;
        private readonly IBlobService _blobService;
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatRoomServices(IChatRoomRepo _repository, IHubContext<ChatHub> hubContext, IBlobService blobService)
        {
            _repo = _repository;
            _hubContext = hubContext;

            _blobService = blobService;
        }

        public async Task<IEnumerable<ChatlistVM>> AddChatRoom(FriendRequest request, int userId)
        {
            return (await _repo.AddChatRoom(request, userId));
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
            /*return await _repo.CreateGroup(createGroupVM.RoomName, createGroupVM.InitiatedBy, selectedUsersTable);*/
           
            // Call AddToGroup to add users to the chat room
            var groupName = createGroupVM.ChatRoomId.ToString();
            foreach (var userId in createGroupVM.SelectedUsers)
            {
                await _hubContext.Clients.Group(groupName).SendAsync("AddToGroup", groupName, userId);
            }
            return chatList;             
        }


        public async Task<bool> UpdateGroupPicture(int ChatRoomId, byte[] fileBytes, string fileName)
        {
            try
            {
                // Upload the file to blob storage and get the URI
                string blobUri = await _blobService.UploadImageFiles(fileBytes, fileName, 2);

                // Update the user's profile picture URI in the database
                int updateResult = await _repo.UpdateGroupPicture(ChatRoomId, blobUri);

                // Assuming the updateResult is an int that signifies the number of records updated
                // You might want to check if it actually succeeded based on your repository implementation
                return updateResult != 0;
            }
            catch (Exception ex)
            {
                // Depending on your logging framework, log the exception
                Console.WriteLine($"An error occurred: {ex.Message}");
                return false;
            }
        }
        public async Task RemoveUserFromGroup(int userId, int chatRoomId, int initiatedBy)
        {
            var groupName = chatRoomId.ToString();
            await _hubContext.Clients.Group(groupName).SendAsync("RemoveFromGroup", groupName, userId, initiatedBy);
        }
    }
}
