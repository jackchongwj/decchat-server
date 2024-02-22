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
        Task<string> GetUserName(int userId);  
        Task<string> GetProfilePictureUrl(byte[] fileByte, string filename);
    }

    public interface IFriendService
    {
        Task<int> AddFriends(Friends friends);                                                              // add new friend
        Task<int> UpdateFriendRequest(FriendRequest request);                                              // update friend request                                                                      // 

    }

    public interface IChatRoomService
    {
        Task<IEnumerable<ChatlistVM>> AddChatRoom(FriendRequest request, int userId);
        Task<int> RemoveUserFromGroup(int chatRoomId, int userId);
        Task<ChatlistVM> CreateGroupWithSelectedUsers(CreateGroupVM createGroupVM);
        Task<bool> UpdateGroupPicture(int ChatRoomId, byte[] fileBytes, string fileName);
        Task<IEnumerable<GroupMember>> RetrieveGroupMemberByChatroomId(int chatRoomId, int userId);
        Task<int> QuitGroup(int chatRoomId, int userId); 


    }

    public interface IMessageService
    {
        Task<int> AddMessages(Messages message);                                                                                 // add new friend 
        Task<IEnumerable<Messages>> GetMessages(int ChatRoomId);
    }

    public interface IAuthService
    {
        Task<string> GetSalt(string username);
        Task<bool> VerifyPassword(string username, string hashedPassword);
        Task<IActionResult> AddUser(Users user);
        Task<bool> ChangePassword(int userId, string currentPassword, string newPassword);
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
