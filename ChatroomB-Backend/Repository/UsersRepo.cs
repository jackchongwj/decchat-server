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
            try
            {
                string sql = "exec GetUserByProfileName @profileName, @userId";

                IEnumerable<UserSearchDetails> result = await _dbConnection.QueryAsync<UserSearchDetails>(sql, new { profileName, userId });

                return result;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to retrieve user details by profile name", ex);
            }
        }

        public async Task<IEnumerable<Users>> GetFriendRequest(int userId)
        {
            try
            {
                string sql = "exec GetFriendsRequest @userId";

                IEnumerable<Users> result = await _dbConnection.QueryAsync<Users>(sql, new { userId });

                return result;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to retrieve friend requests", ex);
            }
        }

        public async Task<Users> GetUserById(int userId)
        {
            try
            {
                string sql = "exec GetUserById @UserId";
                Users user = await _dbConnection.QueryFirstAsync<Users>(sql, new { UserId = userId });
                return user;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to retrieve user details by ID", ex);
            }
        }

        public async Task<int> UpdateProfileName(int userId, string newProfileName)
        {
            try
            {
                string sql = "exec UpdateUserProfileName @UserId, @NewProfileName";
                int result = await _dbConnection.ExecuteAsync(sql, new
                {
                    userId,
                    newProfileName
                });
                return result;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to update profile name", ex);
            }
        }

        public async Task<int> UpdateProfilePicture(int userId, string newProfilePicture)
        {
            try
            {
                string sql = "exec UpdateUserProfilePicture @UserId, @NewProfilePicture";
                int result = await _dbConnection.ExecuteAsync(sql, new
                {
                    userId,
                    newProfilePicture
                });
                return result;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to update profile picture", ex);
            }
        }

        public async Task<int> DeleteUserProfile(int userId)
        {
            try
            {
                string sql = "exec DeleteUserProfile @UserId";
                int result = await _dbConnection.ExecuteAsync(sql, new { UserId = userId });
                return result;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to delete user profile", ex);
            }
        }

        public async Task<bool> DoesUsernameExist(string username)
        {
            try
            {
                string sql = "exec DoesUsernameExist @UserName";
                bool isExist = await _dbConnection.ExecuteScalarAsync<bool>(sql, new { UserName = username });
                return isExist;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to check username existence", ex);
            }
        }

        public async Task<IEnumerable<ChatlistVM>> GetChatListByUserId(int userId)
        {
            try
            {
                DynamicParameters parameter = new DynamicParameters();
                parameter.Add("@UserId", userId);

                string sql = "EXEC RetrieveChatRoomListById @UserId";

                IEnumerable<ChatlistVM> chatList = await _dbConnection.QueryAsync<ChatlistVM>(sql, parameter);

                return chatList.AsList();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to retrieve chat list by user ID", ex);
            }
        }
    }
}
