using ChatroomB_Backend.Models;
using Microsoft.AspNetCore.SignalR;

namespace ChatroomB_Backend.Hubs
{
    public sealed class ChatHub: Hub
    {
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
    }
}
