using ChatroomB_Backend.DTO;
using ChatroomB_Backend.Models;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;
using System.Data;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace ChatroomB_Backend.Service
{
    public interface IUserService
    {
        Task<IEnumerable<UserSearchDetails>> GetByName(string profileName, int userId);                                              //Get user by user profile name and filter friend request
        Task<IEnumerable<Users>> GetFriendRequest(int userId);                                                               //Get All Friend request
        Task<Users> GetUserById(int userId);
        Task<int> UpdateProfileName(int userId, string newProfileName);
        Task<bool> UpdateProfilePicture(int userId, byte[] fileBytes, string fileName);
        Task<int> DeleteUser(int userId);
        
        Task<IEnumerable<ChatlistVM>> GetChatListByUserId(int userId); //return chatlist
        Task<bool> DoesUsernameExist(string username);
        Task<int> GetUserId(string username);
        Task<string> GetProfilePictureUrl(byte[] fileByte, string filename);
    }

    public interface IFriendService
    {
        Task<int> AddFriends(Friends friends);                                                              // add new friend
        Task<int> UpdateFriendRequest(FriendRequest request);                                              // update friend request                                                                      // 

    }

    public interface IChatRoomService
    {
        Task<int> AddChatRoom(FriendRequest request);


        Task CreateGroupWithSelectedUsers(string roomName, int initiatedBy, List<int> SelectedUsers);
    }

    public interface IMessageService
    {
        Task<int> AddMessages(Messages message);                                                                                 // add new friend 
    }

    public interface IAuthService
    {
        Task<string> GetSalt(string username);
        Task<bool> VerifyPassword(string username, string hashedPassword);
        Task<ActionResult> AddUser(Users user);
        Task<bool> ChangePassword(int userId, string currentPassword, string newPassword);
    }

    public interface ITokenService
    {
        Task<ActionResult> StoreRefreshToken(RefreshToken token);
        Task<ActionResult> RemoveRefreshToken(RefreshToken token);
        Task<ActionResult> ValidateRefreshToken(RefreshToken token);
    }

    public interface IBlobService
    {
        Task<string> UploadImageFiles(byte[] fileByte, string filename, int CaseImageFile);
        Task<string> UploadVideoFiles(byte[] vidByte, string vidName);
        Task<string> UploadDocuments(byte[] docByte, string docName);
        Task<string> UploadAudios(byte[] audioByte, string audioName);
        Task DeleteBlob(string blobUri);
    }

    public interface IRedisServcie 
    {
        Task<int> AddUserIdAndConnetionIdToRedis(string userId, string connectionId);                                                  // Add userId and connection id to redis
        Task<int> DeleteUserIdFromRedis(string userId);
        Task<string> SelectUserIdFromRedis(int? userId);
    }
    
}
