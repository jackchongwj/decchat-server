using ChatroomB_Backend.DTO;
using ChatroomB_Backend.Models;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;
using System.Collections.Generic;
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
        Task<string> GetProfilePictureUrl(byte[] fileByte, string filename);
    }

    public interface IFriendService
    {
        Task<IEnumerable<Users>> AddFriends(Friends friends);                                                                   // Add new friend
        Task<int> CheckFriendExist(Friends friends);                                                                            // Check the friend does exist
        Task<int> UpdateFriendRequest(FriendRequest request);                                                                   // Update friend request status(Accept, Reject) 
        Task<int> DeleteFriendRequest(int chatRoomId, int userId1, int userId2);                                                // update the friend reuqest status(Delete)

    }

    public interface IChatRoomService
    {
        Task<IEnumerable<ChatlistVM>> AddChatRoom(FriendRequest request);                                                       // Add new private chatroom
        Task<int> UpdateGroupName(int chatRoomId, string newGroupName);                                                         // Update group chatroom's group name
        Task<int> UpdateGroupPicture(int chatRoomId, byte[] fileBytes, string fileName);                                        // Update group chatroom's profile pic 
        Task<IEnumerable<ChatlistVM>> CreateGroupWithSelectedUsers(CreateGroupVM createGroupVM);                                // Create new group chatroom
        Task<int> RemoveUserFromGroup(int chatRoomId, int userId);                                                              // Remove user from the group chatroom
        Task<IEnumerable<ChatlistVM>> AddMembersToGroup(AddMemberVM addMemberVM);
        Task<IEnumerable<GroupMember>> RetrieveGroupMemberByChatroomId(int chatRoomId, int userId);                             // Get the group member 
        Task<int> QuitGroup(int chatRoomId, int userId);                                                                        // Quit group


    }

    public interface IMessageService
    {
        Task<ChatRoomMessage> AddMessages(Messages message);                                                                    // Add new message
        Task<IEnumerable<ChatRoomMessage>> GetMessages(int ChatRoomId, int MessageId);                                          // Get the message
        Task<int> EditMessage(EditMessage NewMessage);                                                                      // Edit the message
        Task<int> DeleteMessage(int MessageId, int ChatRoomId);                                                                 // Delete the message 
        Task<int> GetTotalSearchMessage(int ChatRoomId, string SearchValue);
    }

    public interface IAuthService
    {
        Task<Users> Authenticate(string username, string password);
        Task AddUser(string username, string password, string profileName);
        Task ChangePassword(string username, string currentPassword, string newPassword);
    }

    public interface ITokenService
    {
        Task ValidateRefreshToken(string token, int userId);
        Task ValidateAccessToken(int userId, string username);
        Task StoreRefreshToken(RefreshToken token);
        Task RemoveRefreshToken(string token);
        Task UpdateRefreshToken(string token);
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
        Task LogError(string controllerName, int userId, string errorMessage);
    }
    
}
