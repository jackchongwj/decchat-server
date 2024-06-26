﻿using ChatroomB_Backend.DTO;
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
        Task<IEnumerable<UserSearchDetails>> GetByName(string profileName, int userId);                                                    
        Task<IEnumerable<Users>> GetFriendRequest(int userId);                                                                 
        Task<Users> GetUserById(int userId);
        Task<int> UpdateProfileName(int userId, string newProfileName);
        Task<int> UpdateProfilePicture(int userId, string newProfilePicture);
        Task<int> DeleteUser(int userId);       
        Task<IEnumerable<ChatlistVM>> GetChatListByUserId(int userId); //return chatlist
        Task<bool> DoesUsernameExist(string username);
    }   

    public interface IFriendRepo
    {
        Task<IEnumerable<Users>> AddFriends(Friends friends);                                                                                    
        Task<int> CheckFriendExist(Friends friends);
        Task<int> UpdateFriendRequest (FriendRequest request);                                            
        Task <int> DeleteFriendRequest(int chatRoomId, int userId1, int userId2);
       
    }

    public interface IChatRoomRepo
    {
        Task <IEnumerable<ChatlistVM>> AddChatRoom(FriendRequest request); 
        Task<int> UpdateGroupName(int chatRoomId, string newGroupName);
        Task<int> UpdateGroupPicture(int chatRoomId, string newGroupPicture);

        Task <IEnumerable<ChatlistVM>> CreateGroup(string roomName, int initiatedBy, DataTable selectedUsers);
        Task<IEnumerable<GroupMember>> RetrieveGroupMemberByChatroomId(int chatRoomId, int userId);
        Task<int> RemoveUserFromGroup (int chatRoomId, int userId);
        Task<int> QuitGroup(int chatRoomId, int userId);
        Task<IEnumerable<ChatlistVM>> AddMembersToGroup(int chatRoomId, DataTable selectedUsers);
        Task<IEnumerable<ChatlistVM>> GetGroupInfoByChatroomId(int chatRoomId, DataTable selectedUsers);

    }

    public interface IMessageRepo
    {
        Task<ChatRoomMessage> AddMessages(ChatRoomMessage message);                                                                               
        Task<IEnumerable<ChatRoomMessage>> GetMessages(int ChatRoomId, int CurrentPage);
        Task<int> EditMessage(EditMessage NewMessage);
        Task<int> DeleteMessage(int MessageId);
        Task<int> GetTotalSearchMessage(int ChatRoomId, string SearchValue);
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
        Task<Users> GetUserCredentials(string username);
        Task<bool> AddUser(Users user);
        Task<bool> ChangePassword(string username, string newHashedPassword);
    }

    public interface ITokenRepo
    {
        Task<bool> ValidateRefreshToken(string token, int userId);
        Task<string> ValidateAccessToken(int userId);
        Task<bool> StoreRefreshToken(RefreshToken token);
        Task<bool> RemoveRefreshToken(string token);
        Task<bool> UpdateRefreshToken(string token);
    }

    public interface IRedisRepo 
    {
        Task<int> AddUserIdAndConnetionIdToRedis(string userId, string connectionId);
        Task<int> DeleteUserIdFromRedis(string userId);
        Task<string> SelectUserIdFromRedis(int? userId);

        Task<List<string>> GetAllUserIdsFromRedisSet();
    }

    public interface IErrorHandleRepo
    {
        Task LogError(string controllerName, int userId, string errorMessage);
    }
}
