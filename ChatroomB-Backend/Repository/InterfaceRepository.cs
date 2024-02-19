﻿using ChatroomB_Backend.DTO;
using ChatroomB_Backend.Models;
using ChatroomB_Backend.Service;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Security.Claims;
using System.Text.RegularExpressions;


namespace ChatroomB_Backend.Repository
{
    public interface IUserRepo
    {
        Task<IEnumerable<UserSearchDetails>> GetByName(string profileName, int userId);                                                    //Get user by user profile name and filter friend request
        Task<IEnumerable<Users>> GetFriendRequest(int userId);                                                                 // Get All Friend request
        Task<Users> GetUserById(int userId);
        Task<int> UpdateUserProfile(Users userProfile);
        Task<int> DeleteUserProfile(int userId);
        Task<int> ChangePassword(int userId, string newPassword);
        Task<IEnumerable<ChatlistVM>> GetChatListByUserId(int userId); //return chatlist
        Task<bool> DoesUsernameExist(string username);
        Task<int> GetUserId(string username);
    }   

    public interface IFriendRepo
    {
        Task<IEnumerable<Users>> AddFriends(Friends friends);                                                                                     // Add new friend 
        Task<int> UpdateFriendRequest (FriendRequest request);                                              // update friend request
        Task <int> DeleteFriendRequest(int chatRoomId, int userId1, int userId2);
       
    }

    public interface IChatRoomRepo
    {
        Task <IEnumerable<ChatlistVM>> AddChatRoom(FriendRequest request, int userId); // add new ChatRoom and user chat room with private user
        Task CreateGroup(string roomName, int initiatedBy, DataTable selectedUsers);
    }

    public interface IMessageRepo
    {
        Task <DTO.Message> AddMessages(Messages message);                                                                               
        Task<IEnumerable<DTO.Message>> GetMessages(int ChatRoomId);
    }
    
    public interface IBlobRepo
    {
        Task<string> UploadImageFiles(byte[] imgByte, string filename, string folderPath);
        Task<string> UploadVideoFiles(byte[] vidByte, string filename, string folderPath);
        Task<string> UploadDocuments(byte[] docByte, string filename, string folderPath);
        Task<string> UploadAudios(byte[] audioByte, string audioName, string folderpath);
        Task DeleteBlob(string blobUri);
    }

    public interface IAuthRepo
    {
        Task<string> GetSalt(string username);
        Task<bool> VerifyPassword(string username, string hashedPassword);
        Task<ActionResult> AddUser(Users user);
    }

    public interface ITokenRepo
    {
        Task<ActionResult> StoreRefreshToken(RefreshToken token);
        Task<ActionResult> RemoveRefreshToken(RefreshToken token);
        Task<ActionResult> ValidateRefreshToken(RefreshToken token);
    }

    public interface IRedisRepo 
    {
        Task<int> AddUserIdAndConnetionIdToRedis(string userId, string connectionId);
        Task<int> DeleteUserIdFromRedis(string userId);
        Task<string> SelectUserIdFromRedis(int? userId);
    }
}
