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

        public async Task<IEnumerable<UserSearch>> GetByName(string profileName, int userId)
        {
            string sql = "exec GetUserByProfileName @profileName, @userId";

            IEnumerable<UserSearch> result = await _dbConnection.QueryAsync<UserSearch>(sql, new {profileName, userId });

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

        public async Task<int> UpdateUserProfile(Users userProfile)
        {
            string sql = "exec UpdateUserProfile @UserId, @NewProfileName, @NewProfilePicture";
            int result = await _dbConnection.ExecuteAsync(sql, new
            {
                UserId = userProfile.UserId,
                NewProfileName = userProfile.ProfileName,
                NewProfilePicture = userProfile.ProfilePicture
            });
            return result;
        }

        public async Task<int> DeleteUserProfile(int userId)
        {
            string sql = "exec DeleteUserProfile @UserId";
            int result = await _dbConnection.ExecuteAsync(sql, new { UserId = userId });
            return result;
        }

        public async Task<bool> DoesUsernameExist(string username)
        {
            try
            {
                string sql = "exec CheckUserNameUnique @UserName";

                bool isUnique = await _dbConnection.ExecuteScalarAsync<bool>(sql, new { UserName = username });

                return isUnique;
            }
            catch
            {
                throw new Exception("An unexpected error occurred");
            }
        }

        public async Task<int> GetUserId(string username)
        {
            try
            {
                string sql = "exec GetUserIdByUserName @username";

                int result = await _dbConnection.ExecuteScalarAsync<int>(sql, new { UserName = username });

                return result;
            }
            catch
            {
                throw new Exception("An unexpected error occurred");
            }
            
        }
        public async Task<int> ChangePassword(int userId, string newPassword)
        {
            string sql = "exec ChangePassword @UserId, @NewPassword";
            int result = await _dbConnection.ExecuteAsync(sql, new { UserId = userId, NewPassword = newPassword });
            return result;
        }

        public async Task<IEnumerable<ChatlistVM>> GetChatListByUserId(int userId)
        {
            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@UserId", userId);

            string sql = "EXEC GetChatListByUserId @UserId";

            IEnumerable<ChatlistVM> chatList = await _dbConnection.QueryAsync<ChatlistVM>(sql, parameter);

            return chatList.AsList();
        }
    }
}
