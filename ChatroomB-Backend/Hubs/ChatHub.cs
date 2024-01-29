using ChatroomB_Backend.DTO;
using ChatroomB_Backend.Service;
using Microsoft.AspNetCore.SignalR;

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
            await Clients.All.SendAsync("aaa", $"{Context.ConnectionId} has joined On Connected");
        }

        // group id 

        public async Task ReceiveIncomingMessage(string message)
        {
            await PassMessage(message);
        }

        public async Task PassMessage(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
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

                IEnumerable<UserSearch> newResult = await GetLatestSearchResults(senderId, profileName);
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

        private async Task<IEnumerable<UserSearch>> GetLatestSearchResults(int userId, string profileName)
        {
            return await services.GetByName(profileName, userId);
        }

    }
}
