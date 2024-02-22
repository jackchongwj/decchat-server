
using Azure.Core;
using ChatroomB_Backend.DTO;
using ChatroomB_Backend.Hubs;
using ChatroomB_Backend.Models;
using ChatroomB_Backend.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Text.RegularExpressions;

namespace ChatroomB_Backend.Service
{
    public class ChatRoomServices : IChatRoomService
    {
        private readonly IChatRoomRepo _repo;
        private readonly IBlobService _blobService;
        private readonly IUserService _userService;
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatRoomServices(IChatRoomRepo _repository, IBlobService blobService, IUserService userService, IHubContext<ChatHub> hubContext)
        {
            _repo = _repository;
            _blobService = blobService;
            _userService = userService;
            _hubContext = hubContext;
        }

        public async Task<IEnumerable<ChatlistVM>> AddChatRoom(FriendRequest request, int userId)
        {
            return (await _repo.AddChatRoom(request, userId));
        }

        public async Task CreateGroupWithSelectedUsers(string roomName, int initiatedBy, List<int> SelectedUsers)
        {
            // Create a DataTable to store the selected user IDs
            DataTable selectedUsersTable = new DataTable();
            selectedUsersTable.Columns.Add("UserId", typeof(int));
            foreach (int userId in SelectedUsers)
            {
                selectedUsersTable.Rows.Add(userId);
            }

            // Call CreateGroup method with the DataTable of selected users
           await _repo.CreateGroup(roomName, initiatedBy, selectedUsersTable);
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

    }
}
