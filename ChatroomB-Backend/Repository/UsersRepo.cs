using ChatroomB_Backend.Data;
using ChatroomB_Backend.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Dapper;
using System.Data;
using System.Collections.Generic;
using ChatroomB_Backend.DTO;

namespace ChatroomB_Backend.Repository
{
    public class UsersRepo : IUserRepo
    {

        private readonly IDbConnection _dbConnection;

        public UsersRepo(IDbConnection db)
        {
            _dbConnection = db;
        }

        public async Task<IEnumerable<UserSearchDetails>> GetByName(string profileName, int userId)
        {
            string sql = "exec GetUserByProfileName @profileName, @userId";

            IEnumerable<UserSearchDetails> result = await _dbConnection.QueryAsync<UserSearchDetails>(sql, new {profileName, userId });

            return result;
        }

        public async Task<IEnumerable<Users>> GetFriendRequest(int userId)
        {
            string sql = "exec GetFriendsRequest @userId";

            IEnumerable<Users> result = await _dbConnection.QueryAsync<Users>(sql, new { userId });

            return result;
        }

        public async Task<Users> GetUserById(int userId)
        {
            string sql = "exec GetUserById @UserId";
            Users users = await _dbConnection.QueryFirstAsync<Users>(sql, new { UserId = userId });
            return users;
        }

        public async Task<int> UpdateProfileName(int userId, string newProfileName)
        {
            string sql = "exec UpdateUserProfileName @UserId, @NewProfileName";
            int result = await _dbConnection.ExecuteAsync(sql, new
            {
                userId,
                newProfileName
            });
            return result;
        }

        public async Task<int> UpdateProfilePicture(int userId, string newProfilePicture)
        {
            string sql = "exec UpdateUserProfilePicture @UserId, @NewProfilePicture";
            int result = await _dbConnection.ExecuteAsync(sql, new
            {
                userId,
                newProfilePicture
            });
            return result;
        }

        public async Task<int> DeleteUser(int userId)
        {
            string sql = "exec DeleteUserProfile @UserId";
            int result = await _dbConnection.ExecuteAsync(sql, new { UserId = userId });
            return result;
        }

        public async Task<bool> DoesUsernameExist(string username)
        {
            string sql = "exec DoesUsernameExist @UserName";
            bool isExist = await _dbConnection.ExecuteScalarAsync<bool>(sql, new { UserName = username });
            return isExist;
        }

        public async Task<int> GetUserId(string username)
        {
            string sql = "exec GetUserIdByUserName @username";
            int result = await _dbConnection.ExecuteScalarAsync<int>(sql, new { UserName = username });
            return result;
        }
        
        public async Task<IEnumerable<ChatlistVM>> GetChatListByUserId(int userId)
        {
            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@UserId", userId);

            string sql = "EXEC RetrieveChatRoomListById @UserId";

            IEnumerable<ChatlistVM> chatList = await _dbConnection.QueryAsync<ChatlistVM>(sql, parameter);

            return chatList.AsList();
        }

        public async Task<string> GetUserName(int userId)
        {
            string sql = "exec GetUserNameByUserId @UserId";
            string result = await _dbConnection.ExecuteScalarAsync<string>(sql, new { UserId = userId });
            return result;
        }
    }
}
