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
        Task<IEnumerable<UserSearch>> GetByName(string profileName, int userId);                                                    //Get user by user profile name and filter friend request
        Task<IEnumerable<Users>> GetFriendRequest(int userId);                                                                 // Get All Friend request
        Task<Users> GetUserById(int userId);
        Task<int> UpdateUserProfile(Users userProfile);
        Task<int> DeleteUserProfile(int userId);
        Task<int> ChangePassword(int userId, string newPassword);
        Task<IEnumerable<ChatlistVM>> GetChatListByUserId(int userId); //return chatlist
        Task<bool> IsUsernameUnique(string username);
        Task<int> GetUserId(string username);
    }   

    public interface IFriendRepo
    {
        Task<int> AddFriends(Friends friends); // Add new friend 
        Task<int> UpdateFriendRequest (FriendRequest request); // update friend request
       
    }

    public interface IChatRoomRepo
    {
        Task<int> AddChatRoom(FriendRequest request); // add new ChatRoom and user chat room with private user
        Task CreateGroup(string roomName, int initiatedBy, DataTable selectedUsers);
    }

    public interface IMessageRepo
    {
        Task<int> AddMessages(Messages message); // add new friend 
    }

    public interface IBlobRepo
    {
        Task<string> UploadImageFiles(byte[] fileByte, string filename, string folderPath);
        Task<string> UploadVideoFiles(string filepath, string filename, string folderPath);
        Task<string> UploadDocuments(string filepath, string filename, string folderPath);
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
    }
}
