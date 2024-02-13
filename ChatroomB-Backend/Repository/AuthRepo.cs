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

        public AuthRepo(IDbConnection db) 
        { 
            _dbConnection = db;
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
            catch (SqlException ex)
            {
                throw new InvalidOperationException("A database error occurred while fetching salt", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An unexpected error occurred", ex);
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
            catch (SqlException ex)
            {
                throw new InvalidOperationException("A database error occurred while verifying password", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An unexpected error occurred", ex);
            }
        }

        public async Task<ActionResult> AddUser(Users user)
        {
            try
            {
                string sql = "exec AddUser @UserName, @HashedPassword, @Salt";

                await _dbConnection.ExecuteAsync(sql, new Users
                {
                    UserName = user.UserName,
                    HashedPassword = user.HashedPassword,
                    Salt = user.Salt
                });

                return new OkObjectResult(new { Messsage = "Registration successful!" });
            }
            catch 
            {
                return new BadRequestObjectResult(new { Error = "Failed to register user" });
            }
        }

        public async Task<bool> ChangePassword(int userId, string newHashedPassword)
        {
            string sql = "exec ChangePassword @UserId, @NewHashedPassword";

            var result = await _dbConnection.ExecuteAsync(
                sql,
                new { UserId = userId, NewHashedPassword = newHashedPassword }
            );

            return true; // Return true if the password was successfully changed
        }

    }
}
