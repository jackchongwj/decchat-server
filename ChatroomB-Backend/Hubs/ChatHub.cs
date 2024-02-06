using ChatroomB_Backend.DTO;
using ChatroomB_Backend.Service;
using Microsoft.AspNetCore.SignalR;
using NuGet.Protocol.Plugins;
using System.Collections;
using System.Collections.Generic;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

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

        public async Task acceptFriendRequest(int chatroomId, int userId) 
        {
            try
            {

                await Clients.User(userId.ToString()).SendAsync("ReceiveFriendRequestNotification");

                await AddToGroup(null,chatroomId, userId);
                IEnumerable<ChatlistVM> newResult = await _Uservices.GetChatListByUserId(userId);
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

        public async Task AddToGroup(List<ChatlistVM>? chatlists, int? chatRoomId, int? userId)
        {
            if (chatlists!= null)
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

        public async Task RemoveFromGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

            await Clients.Group(groupName).SendAsync("Send", $"{Context.ConnectionId} has left the group {groupName}.");
        }

    }
}
