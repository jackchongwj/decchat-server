using ChatroomB_Backend.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace ChatroomB_Backend.Repository
{
    public class AuthRepo : IAuthRepo
    {
        private readonly IDbConnection _dbConnection;
        private readonly IConfiguration _config;


        public AuthRepo(IDbConnection db, IConfiguration config)
        {
            _dbConnection = db;
            _config = config;
        }

        public async Task<Users> GetUserCredentials(string username)
        {
            try
            {
                string sql = "exec GetUserCredentials @UserName";

                Users user = await _dbConnection.QuerySingleOrDefaultAsync<Users>(sql, new { UserName = username });

                return user;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to get user credentials", ex);
            }
        }

        public async Task<string> GetSalt(string username)
        {
            try
            {
                string sql = "exec GetSaltByUserName @UserName";

                string salt = await _dbConnection.ExecuteScalarAsync<string>(sql, new { UserName = username });

                if (salt == null)

                {
                    throw new ArgumentNullException("Salt not found for the user");
                }

                return salt;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to get salt for user", ex);
            }
        }

        public async Task<bool> VerifyPassword(string username, string hashedPassword)
        {
            try
            {
                string sql = "exec VerifyUserCredentials @UserName, @HashedPassword";

                int result = await _dbConnection.ExecuteScalarAsync<int>(sql, new { UserName = username, HashedPassword = hashedPassword });

                return result == 1;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to verify password", ex);
            }
        }

        public async Task<bool> AddUser(Users user)
        {
            try
            {
                string sql = "exec AddUser @UserName, @HashedPassword, @Salt, @ProfileName, @ProfilePicture";

                await _dbConnection.ExecuteAsync(sql, new Users
                {
                    UserName = user.UserName,
                    ProfileName = user.ProfileName,
                    HashedPassword = user.HashedPassword,
                    Salt = user.Salt,
                    ProfilePicture = _config["DefaultPicture:UserProfile"]
                });

                return true;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to register user", ex);
            }
        }

        public async Task<bool> ChangePassword(int userId, string newHashedPassword)
        {
            try
            {
                string sql = "exec ChangePassword @UserId, @NewHashedPassword";

                await _dbConnection.ExecuteAsync(sql, new { UserId = userId, NewHashedPassword = newHashedPassword });

                return true; 
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to change password", ex);
            }
        }
    }
}

