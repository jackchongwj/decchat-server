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

                Users? user = await _dbConnection.QuerySingleOrDefaultAsync<Users>(sql, new { UserName = username });

                return user!;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to get user credentials", ex);
            }
        }

        public async Task<bool> AddUser(Users user)
        {
            try
            {
                string sql = "exec AddUser @UserName, @HashedPassword, @Salt, @ProfileName, @ProfilePicture, @Success OUTPUT";

                var parameters = new DynamicParameters(user);
                parameters.Add("@Success", dbType: DbType.Boolean, direction: ParameterDirection.Output);
                parameters.Add("@ProfilePicture", _config["DefaultPicture:UserProfile"]); // Assuming you still want to set this from the config

                await _dbConnection.ExecuteAsync(sql, parameters);

                return parameters.Get<bool>("@Success");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to register user", ex);
            }
        }

        public async Task<bool> ChangePassword(string username, string newHashedPassword)
        {
            try
            {
                string sql = "exec ChangePassword @UserName, @NewHashedPassword, @RowsAffected OUTPUT";

                var parameters = new DynamicParameters();
                parameters.Add("@UserName", username);
                parameters.Add("@NewHashedPassword", newHashedPassword);
                parameters.Add("@RowsAffected", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await _dbConnection.ExecuteAsync(sql, parameters);

                int rowsAffected = parameters.Get<int>("@RowsAffected");
                return rowsAffected > 0; // Returns true if one or more rows were affected, indicating success.
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to change password", ex);
            }
        }
    }
}

