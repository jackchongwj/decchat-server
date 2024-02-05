using ChatroomB_Backend.DTO;
using ChatroomB_Backend.Service;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace ChatroomB_Backend.Hubs
{
    public sealed class ChatHub: Hub
    {
        private readonly IUserService _Uservices;
        private readonly IRedisServcie _RServices;

        public ChatHub(IUserService _UserService, IRedisServcie rServices)
        {
            _Uservices = _UserService;
            _RServices = rServices;
        }

        public override async Task OnConnectedAsync()
        {
            //string? userId = Context.User?.Identity?.Name;
            string userId = "7";
            string connectionId = Context.ConnectionId;
            Console.WriteLine($"Connection ID {connectionId} connected.");

            //await Clients.All.SendAsync("aaa", $"{Context.ConnectionId} has joined On Connected");

            await _RServices.AddUserIdAndConnetionIdToRedis(userId, connectionId);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception) 
        
        {
            //string? userId = Context.User?.Identity?.Name;
            string userId = "7";
            string connectionId = Context.ConnectionId;

            await _RServices.DeleteUserIdFromRedis(userId);

            await base.OnDisconnectedAsync(exception);
            
        }

        public async Task CheckUserTyping(string name, bool typing)
        {
            await Clients.All.SendAsync("UserTyping", name, typing);
        }
        //public async Task SendFriendRequestNotification(IEnumerable<Friends> friend)
        //{

        //    await Clients.User(friend.FirstOrDefault().ReceiverId.ToString()).SendAsync("ReceiveFriendRequestNotification", friend.FirstOrDefault().SenderId);
        //}

        public async Task SendFriendRequestNotification(int receiverId, int senderId, string profileName)
        {
            try
            {

                await Clients.User(receiverId.ToString()).SendAsync("ReceiveFriendRequestNotification");

                IEnumerable<UserSearch> newResult = await _Uservices.GetByName(profileName, senderId);
                //await Clients.Caller.SendAsync("UpdateSearchResults", newResult);
                //await Clients.User(senderId.ToString()).SendAsync("UpdateSearchResults", newResult);
                //await Clients.User(receiverId.ToString()).SendAsync("UpdateSearchResults", newResult);
                await Clients.All.SendAsync("UpdateSearchResults", newResult);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error in SendFriendRequestNotification: {ex.Message}");
                throw;
            }
        }

        //private async Task<IEnumerable<UserSearch>> GetLatestSearchResults(int userId, string profileName)
        //{
        //    return await services.GetByName(profileName, userId);
        //}



        private async Task AddToGroup(string UserConnectionId, int ChatRoomId)
        {
            await Groups.AddToGroupAsync(UserConnectionId, $"ChatRoomId");
        }

    }
}
