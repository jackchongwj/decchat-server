
using Azure.Core;
using ChatroomB_Backend.DTO;
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
            // assume return chatroomid for new group
            await _repo.CreateGroup(roomName, initiatedBy, selectedUsersTable);

            // foreach (let userid of SelectedUsers)
            // AddToGroup(chatlists = null, chatRoomId = , userId = userid)
        }
    }
}
