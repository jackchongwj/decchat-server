
using Azure.Core;
using ChatroomB_Backend.DTO;
using ChatroomB_Backend.Hubs;
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
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IRedisServcie _RServices;

        public ChatRoomServices(IChatRoomRepo _repository, IHubContext<ChatHub> hubContext, IRedisServcie rServices)
        {
            _repo = _repository;
            _hubContext = hubContext;
            _RServices = rServices;
        }

        public async Task<IEnumerable<ChatlistVM>> AddChatRoom(FriendRequest request, int userId)
        {
            IEnumerable<ChatlistVM> result = await _repo.AddChatRoom(request, userId);

            if(!result.IsNullOrEmpty()) 
            {
                try 
                {
                    // add private list to signalR group for send message
                    string connectionId = await _RServices.SelectUserIdFromRedis(userId);
                    await _hubContext.Groups.AddToGroupAsync(connectionId, "PG"+ result.Select(list => list.ChatRoomId).First());


                    await _hubContext.Clients.Group("FR"+ request.ReceiverId).SendAsync("UpdatePrivateChatlist", result.ElementAt(0));
                    await _hubContext.Clients.Group("FR"+ request.SenderId).SendAsync("UpdatePrivateChatlist", result.ElementAt(1));
                } 
                catch (Exception ex) 
                {

                    Console.Error.WriteLine($"Error in UpdatePrivateChatlist: {ex.Message}");
                    throw;
                }
            }

            return result.ToList();
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
    }
}
