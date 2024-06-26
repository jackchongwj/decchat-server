﻿using ChatroomB_Backend.DTO;
using ChatroomB_Backend.Models;
using ChatroomB_Backend.Service;
using ChatroomB_Backend.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using MongoDB.Driver.Core.Connections;
using NuGet.Protocol.Plugins;
using System.Collections;
using System.Collections.Generic;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ChatroomB_Backend.Hubs
{
    public sealed class ChatHub: Hub
    {
        private readonly IUserService _Uservices;
        private readonly IRedisServcie _RServices;
        private readonly IAuthUtils _authUtils;

        public ChatHub(IUserService _UserService, IRedisServcie _RedisServices, IAuthUtils authUtils)
        {
            _Uservices = _UserService;
            _RServices = _RedisServices;
            _authUtils = authUtils;
        }

        [Authorize]
        //SignalR start and destroy connection
        public override async Task OnConnectedAsync()
        {
            try
            {
                Claim userIdClaim = Context.User.Claims.FirstOrDefault(c => c.Type == "UserId")!;
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    // Handle the case where the userId is not found or not an integer
                    throw new UnauthorizedAccessException("User ID claim is not present or invalid in the token");
                }

                // declare userId and connection Id
                string connectionId = Context.ConnectionId;

                // call redis to add userId and Connection id to redis
                await _RServices.AddUserIdAndConnetionIdToRedis(userId.ToString(), connectionId);

                

                //add user to a group to easy call them
                string groupName = "User" + userId.ToString();
                await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

                // add list to group 
                await AddToGroup(Convert.ToInt32(userId.ToString()));

                await base.OnConnectedAsync();

            }
            catch (Exception ex) 
            {
                Console.Error.WriteLine($"Error in Redis Connection method: {ex.ToString()}");
                throw;
            }
        }

        [Authorize]
        public override async Task OnDisconnectedAsync(Exception? exception) 
        
        {
            try
            {
                int userId = _authUtils.ExtractUserIdFromJWT(Context.User);

                string connectionId = Context.ConnectionId;


                // Notify other members in the user's groups that they have gone offline
                // retrieve the list of groups or chat rooms the user is part of
                IEnumerable<ChatlistVM> chatlist = await _Uservices.GetChatListByUserId(userId);
                foreach (var list in chatlist)
                {
                    await Clients.Group(list.ChatRoomId.ToString()).SendAsync("UpdateUserOnlineStatus", userId.ToString(), false);
                    await Groups.RemoveFromGroupAsync(connectionId, list.ChatRoomId.ToString());
                }
                await _RServices.DeleteUserIdFromRedis(userId.ToString()!);

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

