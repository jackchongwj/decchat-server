using ChatroomB_Backend.DTO;
using ChatroomB_Backend.Models;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;
using System.Data;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ChatroomB_Backend.Service
{
    public interface IUserService
    {
        Task<IEnumerable<UserSearchDetails>> GetByName(string profileName, int userId);
        Task<IEnumerable<Users>> GetFriendRequest(int userId);
        Task<Users> GetUserById(int userId);
        Task<int> UpdateProfileName(int userId, string newProfileName);
        Task<int> UpdateProfilePicture(int userId, byte[] fileBytes, string fileName);
        Task<int> DeleteUser(int userId);
        
        Task<IEnumerable<ChatlistVM>> GetChatListByUserId(int userId);
        Task<bool> DoesUsernameExist(string username);
        Task<int> GetUserId(string username);
        Task<string> GetUserName(int userId);  
        Task<string> GetProfilePictureUrl(byte[] fileByte, string filename);
    }

    public interface IFriendService
    {
        Task<IEnumerable<Users>> AddFriends(Friends friends); 
        Task<int> CheckFriendExit(Friends friends);
        Task<int> UpdateFriendRequest(FriendRequest request); 
        Task<int> DeleteFriendRequest(int chatRoomId, int userId1, int userId2);

    }

    public interface IChatRoomService
    {
        Task<IEnumerable<ChatlistVM>> AddChatRoom(FriendRequest request, int userId);
        Task<int> UpdateGroupName(int chatRoomId, string newGroupName);
        Task<int> UpdateGroupPicture(int chatRoomId, byte[] fileBytes, string fileName);
        Task<IEnumerable<ChatlistVM>> CreateGroupWithSelectedUsers(CreateGroupVM createGroupVM);
        Task<int> RemoveUserFromGroup(int chatRoomId, int userId);
        Task<IEnumerable<ChatlistVM>> AddMembersToGroup(AddMemberVM addMemberVM);
        Task<IEnumerable<GroupMember>> RetrieveGroupMemberByChatroomId(int chatRoomId, int userId);
        Task<int> QuitGroup(int chatRoomId, int userId); 


    }

    public interface IMessageService
    {
        Task<ChatRoomMessage> AddMessages(Messages message); 
        Task<IEnumerable<ChatRoomMessage>> GetMessages(int ChatRoomId, int MessageId);
        Task<int> EditMessage(ChatRoomMessage NewMessage);
        Task<int> DeleteMessage(int MessageId, int ChatRoomId);
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
        Task<string> RenewAccessToken(RefreshToken token, int userId, string username);
        Task<IActionResult> StoreRefreshToken(RefreshToken token);
        Task<IActionResult> RemoveRefreshToken(RefreshToken token);
        Task<IActionResult> UpdateRefreshToken(RefreshToken token);
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
        Task<int> AddUserIdAndConnetionIdToRedis(string userId, string connectionId);
        Task<int> DeleteUserIdFromRedis(string userId);
        Task<string> SelectUserIdFromRedis(int? userId);
        Task<List<string>> GetAllUserIdsFromRedisSet();
    }

    public interface IErrorHandleService 
    {
        Task LogError(string controllerName, string errorMessage);
    }
    
}
