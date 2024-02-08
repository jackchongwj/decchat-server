﻿using ChatroomB_Backend.DTO;
using ChatroomB_Backend.Models;
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
            try
            {
                // declare userId and connection Id
                string userId = Context.GetHttpContext().Request.Query["userId"];
                string connectionId = Context.ConnectionId;

                Console.WriteLine($"Connection ID {connectionId} connected.");

                // call redis to add userId and Connection id to redis
                await _RServices.AddUserIdAndConnetionIdToRedis(userId, connectionId);

                //add user to a group to easy call them
                await Groups.AddToGroupAsync(Context.ConnectionId, "FR"+ userId);

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

                await _RServices.DeleteUserIdFromRedis(userId);

                await base.OnDisconnectedAsync(exception);
            }
            catch (Exception ex) 
            {
                Console.Error.WriteLine($"Error in Redis Connection method: {ex.ToString()}");
                throw;
            }
        }

        public async Task SendFriendRequestNotification(int receiverId, int senderId, string profileName)
        {
            try
            {
                IEnumerable<Users> GetFriendRequest = await _Uservices.GetFriendRequest(receiverId);

                await Clients.Group("FR"+ receiverId.ToString()).SendAsync("UpdateSearchResults", senderId);
                await Clients.Group("FR"+ senderId.ToString()).SendAsync("UpdateSearchResults", receiverId);
                await Clients.Group("FR" + receiverId.ToString()).SendAsync("UpdateFriendRequest", GetFriendRequest);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error in SendFriendRequestNotification: {ex.Message}");
                throw;
            }
        }

        public async Task acceptFriendRequest(int chatroomId, int senderId,int receiverId) 
        {
            try
            {
                await AddToGroup(null,chatroomId, senderId);
                IEnumerable<ChatlistVM> newResult = await _Uservices.GetChatListByUserId(senderId);
                await Clients.Group("FR"+ senderId.ToString()).SendAsync("UpdateSearchResultsAfterAccept", receiverId);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error in SendFriendRequestNotification: {ex.Message}");
                throw;
            }
        }

        public async Task rejectFriendRequest(int senderId, int receiverId)
        {
            try
            {
                await Clients.Group("FR"+ senderId.ToString()).SendAsync("UpdateSearchResultsAfterReject", receiverId);
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
