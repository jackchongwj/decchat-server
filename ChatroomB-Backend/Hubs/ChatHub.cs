using ChatroomB_Backend.DTO;
using ChatroomB_Backend.Models;
using ChatroomB_Backend.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using MongoDB.Driver.Core.Connections;
using NuGet.Protocol.Plugins;
using System.Collections;
using System.Collections.Generic;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ChatroomB_Backend.Hubs
{
    public sealed class ChatHub: Hub
    {
        private readonly IUserService _Uservices;
        private readonly IMessageService _MServices;
        private readonly IRedisServcie _RServices;
        private readonly IChatRoomService _ChatRoomService;

        public ChatHub(IChatRoomService ChatRoomService, IUserService _UserService, IMessageService _MessageService, IRedisServcie _RedisServices)
        {
            _Uservices = _UserService;
            _RServices = _RedisServices;
            _MServices = _MessageService;
            _ChatRoomService = ChatRoomService;
        }

        //SignalR start and destroy connection
        public override async Task OnConnectedAsync()
        {
            try
            {
                // declare userId and connection Id
                string userId = Context.GetHttpContext().Request.Query["userId"];
                string connectionId = Context.ConnectionId;

                // call redis to add userId and Connection id to redis
                await Task.Delay(500);
                await _RServices.AddUserIdAndConnetionIdToRedis(userId, connectionId);

                

                //add user to a group to easy call them
                string groupName = "User" + userId.ToString();
                await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

                // add list to group 
                await AddToGroup(Convert.ToInt32(userId));

                

                await base.OnConnectedAsync();

            }
            catch (Exception ex) 
            {
                Console.Error.WriteLine($"Error in Redis Connection method: {ex.ToString()}");
                throw;
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception) 
        
        {
            try
            {
                string userId = Context.GetHttpContext().Request.Query["userId"];
                string connectionId = Context.ConnectionId;


                // Notify other members in the user's groups that they have gone offline
                // retrieve the list of groups or chat rooms the user is part of
                IEnumerable<ChatlistVM> chatlist = await _Uservices.GetChatListByUserId(Convert.ToInt32(userId));
                foreach (var list in chatlist)
                {
                    await Clients.Group(list.ChatRoomId.ToString()).SendAsync("UpdateUserOnlineStatus", userId, false);
                    await Groups.RemoveFromGroupAsync(connectionId, list.ChatRoomId.ToString());
                    Console.WriteLine($"{connectionId} has left the group {list.ChatRoomId}");

                    // Broadcast user's offline status to the group
                }
                await _RServices.DeleteUserIdFromRedis(userId);

                await base.OnDisconnectedAsync(exception);
            }
            catch (Exception ex) 
            {
                Console.Error.WriteLine($"Error in Redis Connection method: {ex.ToString()}");
                throw;
            }
        }

        public async Task CheckUserTyping(int ChatRoomId, bool typing, string profilename)
        {

            await Clients.OthersInGroup(ChatRoomId.ToString()).SendAsync("UserTyping", ChatRoomId, typing, profilename);
        }

        public async Task AddToGroup(int userId) 
        {
            try
            {
                IEnumerable<ChatlistVM> chatlist = await _Uservices.GetChatListByUserId(Convert.ToInt32(userId));

                if (chatlist.Any())
                {
                    foreach (var list in chatlist)
                    {
                        await Groups.AddToGroupAsync(Context.ConnectionId, list.ChatRoomId.ToString());

                        await Clients.Group(list.ChatRoomId.ToString()).SendAsync("UpdateUserOnlineStatus", userId, true);
                    }
                    await Clients.Group("User" + userId).SendAsync("Chatlist", chatlist);
                }

            }
            catch (Exception ex) 
            {
                Console.Error.WriteLine($"Error in Redis Connection method: {ex.ToString()}");
                throw;
            }
        }

    }
}

