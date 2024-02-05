using ChatroomB_Backend.DTO;
using ChatroomB_Backend.Service;
using Microsoft.AspNetCore.SignalR;
using System;

namespace ChatroomB_Backend.Hubs
{
    public sealed class ChatHub: Hub
    {
        private readonly IUserService services;

        public ChatHub(IUserService _UserService)
        {
            services = _UserService;
        }

        public override async Task OnConnectedAsync()
        {
            // Add UserId + Associate ConnectionID to Redis
            string con = Context.ConnectionId;
            await Clients.All.SendAsync("ReceiveMessage", $"{Context.ConnectionId} has joined On Connected");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? Exception)
        {
            // Remove the List in Redis
            await base.OnDisconnectedAsync(Exception);
        }

        public async Task CheckUserTyping(string name, bool typing)
        {
            await Clients.All.SendAsync("UserTyping", name, typing);
        }


        public async Task SendFriendRequestNotification(int receiverId, int senderId, string profileName)
        {
            try
            {

                await Clients.User(receiverId.ToString()).SendAsync("ReceiveFriendRequestNotification");

                IEnumerable<UserSearch> newResult = await GetLatestSearchResults(senderId, profileName);

                await Clients.All.SendAsync("UpdateSearchResults", newResult);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error in SendFriendRequestNotification: {ex.Message}");
                throw;
            }
        }

        private async Task<IEnumerable<UserSearch>> GetLatestSearchResults(int userId, string profileName)
        {
            return await services.GetByName(profileName, userId);
        }

        private async Task AddToGroup(string UserConnectionId, int ChatRoomId)
        {
            await Groups.AddToGroupAsync(UserConnectionId, $"ChatRoomId");
        }

    }
}
