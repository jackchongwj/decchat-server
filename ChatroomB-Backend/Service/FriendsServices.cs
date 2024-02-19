﻿using ChatroomB_Backend.DTO;
using ChatroomB_Backend.Hubs;
using ChatroomB_Backend.Models;
using ChatroomB_Backend.Repository;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using NuGet.Protocol.Plugins;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ChatroomB_Backend.Service
{
    public class FriendsServices : IFriendService
    {
        private readonly IFriendRepo _repo;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IRedisServcie _RServices;

        public FriendsServices(IFriendRepo _repository, IHubContext<ChatHub> hubContext, IRedisServcie _RedisServices) 
        {
            _repo = _repository;
            _hubContext = hubContext;
            _RServices = _RedisServices;
        }

        public async Task<IEnumerable<Users>> AddFriends(Friends friends)
        {
            IEnumerable<Users> result = await _repo.AddFriends(friends);

            if(!result.IsNullOrEmpty()) 
            {
                try
                {
                    await _hubContext.Clients.Group("FR"+friends.ReceiverId.ToString()).SendAsync("UpdateSearchResults", friends.SenderId);
                    await _hubContext.Clients.Group("FR" + friends.SenderId.ToString()).SendAsync("UpdateSearchResults", friends.ReceiverId);
                    await _hubContext.Clients.Group("FR" + friends.ReceiverId.ToString()).SendAsync("UpdateFriendRequest", result);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Error in SendFriendRequestNotification: {ex.Message}");
                    throw;
                }
            }

            return result;
        }

        public async Task<int> UpdateFriendRequest(FriendRequest request)
        {
            int result = await _repo.UpdateFriendRequest(request);

            if (result == 1) 
            {
                try
                {
                    string groupName = "FR" + request.SenderId.ToString();
                    string methodName = "";

                    if (request.Status == 2)
                    {
                        methodName = "UpdateSearchResultsAfterAccept";
                    }
                    else {
                        methodName = "UpdateSearchResultsAfterReject";
                    }

                    await _hubContext.Clients.Group(groupName).SendAsync(methodName, request.ReceiverId);
                }catch (Exception ex) 
                {
                    Console.Error.WriteLine($"Error in UpdateFriendRequest: {ex.Message}");
                    throw;
                }

            }
            return result;
        }


        public async Task<int> DeleteFriendRequest(int chatRoomId, int userId1, int userId2)
        {
           int result = await _repo.DeleteFriendRequest(chatRoomId, userId1, userId2);

            if(result == 1)
            {
                //remove member from signalR group
                string connectionIdUser1 = await _RServices.SelectUserIdFromRedis(userId1);
                string connectionIdUser2 = await _RServices.SelectUserIdFromRedis(userId2);
                await _hubContext.Groups.RemoveFromGroupAsync(connectionIdUser1, chatRoomId.ToString());
                await _hubContext.Groups.RemoveFromGroupAsync(connectionIdUser2, chatRoomId.ToString());


                await _hubContext.Clients.Group("FR"+ userId1).SendAsync("DeleteFriend", userId2);
                await _hubContext.Clients.Group("FR"+ userId2).SendAsync("DeleteFriend", userId1);

            }
           return result;
        }
    }
}
