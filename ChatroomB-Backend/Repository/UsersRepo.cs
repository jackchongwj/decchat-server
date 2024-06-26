﻿using ChatroomB_Backend.Data;
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
                throw new Exception("Failed to execute GetByName", ex);
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
                throw new Exception("Failed to execute GetFriendRequest", ex);
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
                throw new Exception("Failed to execute GetUserById", ex);
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
                throw new Exception("Failed to execute UpdateProfileName", ex);
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
                throw new Exception("Failed to execute UpdateProfilePicture", ex);
            }
        }

        public async Task<int> DeleteUser(int userId)
        {
            try
            {
                string sql = "exec DeleteUserProfile @UserId";
                int result = await _dbConnection.ExecuteAsync(sql, new { UserId = userId });
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to execute DeleteUser", ex);
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
                throw new Exception("Failed to execute DoesUsernameExist", ex);
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
                throw new Exception("Failed to execute GetChatListByUserId", ex);
            }
        }
    }
}
