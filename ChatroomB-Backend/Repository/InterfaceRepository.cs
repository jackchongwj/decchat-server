using ChatroomB_Backend.DTO;
using ChatroomB_Backend.Models;
using ChatroomB_Backend.Service;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace ChatroomB_Backend.Repository
{
    public interface IUserRepo
    {
        Task<IEnumerable<UserSearchDetails>> GetByName(string profileName, int userId);                                                    //Get user by user profile name and filter friend request
        Task<IEnumerable<Users>> GetFriendRequest(int userId);                                                                 // Get All Friend request
        Task<Users> GetUserById(int userId);
        Task<int> UpdateProfileName(int userId, string newProfileName);
        Task<int> UpdateProfilePicture(int userId, string newProfilePicture);
        Task<int> DeleteUserProfile(int userId);       
        Task<IEnumerable<ChatlistVM>> GetChatListByUserId(int userId); //return chatlist
        Task<bool> DoesUsernameExist(string username);
        Task<int> GetUserId(string username);
        Task<string> GetUserName(int userId);
    }   

    public interface IFriendRepo
    {
        Task<IEnumerable<Users>> AddFriends(Friends friends);                                                                                     // Add new friend 
        Task<int> CheckFriendExit(Friends friends);
        Task<int> UpdateFriendRequest (FriendRequest request);                                              // update friend request
        Task <int> DeleteFriendRequest(int chatRoomId, int userId1, int userId2);
       
    }

    public interface IChatRoomRepo
    {
        Task <IEnumerable<ChatlistVM>> AddChatRoom(FriendRequest request, int userId); // add new ChatRoom and user chat room with private user
        Task<int> UpdateGroupName(int chatRoomId, string newGroupName);
        Task<int> UpdateGroupPicture(int chatRoomId, string newGroupPicture);

        Task <IEnumerable<ChatlistVM>> CreateGroup(string roomName, int initiatedBy, DataTable selectedUsers);
        Task<IEnumerable<GroupMember>> RetrieveGroupMemberByChatroomId(int chatRoomId, int userId);
        Task<int> RemoveUserFromGroup (int chatRoomId, int userId);
        Task<int> QuitGroup(int chatRoomId, int userId);

    }

    public interface IMessageRepo
    {
        Task<ChatRoomMessage> AddMessages(Messages message);                                                                               
        Task<IEnumerable<ChatRoomMessage>> GetMessages(int ChatRoomId);
        Task<int> EditMessage(ChatRoomMessage NewMessage);
        Task<int> DeleteMessage(int MessageId);
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
        Task<IActionResult> AddUser(Users user);
        Task<bool> ChangePassword(int userId, string newHashedPassword);
    }

    public interface ITokenRepo
    {
        Task<bool> IsRefreshTokenValid(RefreshToken token);
        Task<IActionResult> StoreRefreshToken(RefreshToken token);
        Task<IActionResult> RemoveRefreshToken(RefreshToken token);
    }

    public interface IRedisRepo 
    {
        Task<int> AddUserIdAndConnetionIdToRedis(string userId, string connectionId);
        Task<int> DeleteUserIdFromRedis(string userId);
        Task<string> SelectUserIdFromRedis(int? userId);
    }

    public interface IErrorHandleRepo
    {
        Task LogError(string controllerName, string errorMessage);
    }
}
