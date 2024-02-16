
using Azure.Core;
using ChatroomB_Backend.DTO;
using ChatroomB_Backend.Hubs;
using ChatroomB_Backend.Models;
using ChatroomB_Backend.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Text.RegularExpressions;

namespace ChatroomB_Backend.Service
{
    public class ChatRoomServices : IChatRoomService
    {
        private readonly IChatRoomRepo _repo;

        public ChatRoomServices(IChatRoomRepo _repository)
        {
            _repo = _repository;
        }

        public async Task<int> AddChatRoom(FriendRequest request)
        {
            return (await _repo.AddChatRoom(request));
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
           return await _repo.CreateGroup(createGroupVM.RoomName, createGroupVM.InitiatedBy, selectedUsersTable);
        }

        /* // Call the CreateGroup method and then AddToGroup method
         public async Task CreateGroupWithSelectedUsers(string roomName, int initiatedBy, List<int> SelectedUsers)
         {
             try
             {
                 // Create a DataTable to store the selected user IDs
                 DataTable selectedUsersTable = new DataTable();
                 selectedUsersTable.Columns.Add("UserId", typeof(int));
                 foreach (int userId in SelectedUsers)
                 {
                     selectedUsersTable.Rows.Add(userId);
                 }

                 // Call CreateGroup to get the chatRoomID
                 int chatRoomID = await _repo.CreateGroup(roomName, initiatedBy, selectedUsersTable);

                 // Call AddToGroup to add users to the chat room
                 await _repo.AddToGroup(chatRoomID, SelectedUsers);
             }
             catch (Exception ex)
             {
                 Console.WriteLine("Error: " + ex.Message);
                 throw;
             }
         }

         // Modify the AddToGroup method to accept chatRoomID and selectedUsersList
         public async Task AddToGroup(int chatRoomId, List<int> selectedUsers)
         {
             if (chatlists != null)
             {
                 foreach (var list in chatlists)
                 {
                     string groupName = list.ChatRoomId.ToString();
                     await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

                     await Clients.GroupExcept(groupName, Context.ConnectionId).SendAsync("Send", $"{Context.ConnectionId} has joined the group {groupName}.");
                 }
             }
             else
             {
                 string connectionId = await _RServices.SelectUserIdFromRedis(userId);
                 string groupName = chatRoomId.ToString();

                 if (connectionId != "")
                 {
                     await Groups.AddToGroupAsync(connectionId, groupName);
                     await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
                     await Clients.GroupExcept(groupName, connectionId).SendAsync("Send", $"{connectionId} has joined the group {groupName}.");
                 }
                 else
                 {
                     await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
                 }
             }
         }
 */
    }
}
