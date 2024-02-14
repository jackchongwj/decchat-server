﻿using ChatroomB_Backend.DTO;
using ChatroomB_Backend.Models;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;
using System.Security.Claims;

namespace ChatroomB_Backend.Service
{
    public interface IUserService
    {
        Task<IEnumerable<UserSearch>> GetByName(string profileName, int userId);                                              //Get user by user profile name and filter friend request
        Task<IEnumerable<Users>> GetFriendRequest(int userId);                                                               //Get All Friend request
        Task<Users> GetUserById(int userId);
        Task<int> UpdateUser(Users user);
        Task<int> DeleteUser(int userId);
        Task<int> ChangePassword(int userId, string newPassword);
        Task<IEnumerable<ChatlistVM>> GetChatListByUserId(int userId); //return chatlist
        Task<bool> DoesUsernameExist(string username);
        Task<int> GetUserId(string username);
    }

    public interface IFriendService 
    {
        Task<int> AddFriends(Friends friends);                                                              // add new friend
        Task<int> UpdateFriendRequest(FriendRequest request);                                              // update friend request                                                                      // 

    }

    public interface IChatRoomService
    {
        Task<int> AddChatRoom(FriendRequest request);                                             // add new ChatRoom and user chat room with private user
    }


    public interface IMessageService
    {
        Task<int> AddMessages(Messages message);                                                                                 // add new friend 
    }

    public interface IAuthService
    {
        Task<string> GetSalt(string username);
        Task<bool> VerifyPassword(string username, string hashedPassword);
        Task<IActionResult> AddUser(Users user);
    }

    public interface ITokenService
    {
        Task<string> RenewAccessToken(RefreshToken token, int userId);
        Task<IActionResult> StoreRefreshToken(RefreshToken token);
        Task<IActionResult> RemoveRefreshToken(RefreshToken token);
        Task<IActionResult> ValidateRefreshToken(RefreshToken token);
    }

    public interface IBlobService
    {
        Task<string> UploadImageFiles(byte[] fileByte, string filename, int CaseImageFile);
        Task<string> UploadVideoFiles(string filepath);
        Task<string> UploadDocuments(string filepath);
        Task DeleteBlob(string blobUri);
    }

    public interface IRedisServcie 
    {
        Task<int> AddUserIdAndConnetionIdToRedis(string userId, string connectionId);                                                  // Add userId and connection id to redis
        Task<int> DeleteUserIdFromRedis(string userId);
        Task<string> SelectUserIdFromRedis(int? userId);
    }
    
}
