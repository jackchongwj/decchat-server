using ChatroomB_Backend.Models;
using Dapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.Data;

namespace ChatroomB_Backend.Repository
{
    public class TokenRepo : ITokenRepo

    {
        private readonly IDbConnection _dbConnection;
        private readonly IConfiguration _config;

        public TokenRepo(IDbConnection db, IConfiguration config)
        {
            _dbConnection = db;
            _config = config;
        }

        public async Task<bool> ValidateRefreshToken(string token, int userId)
        {
            string connectionString = _config.GetConnectionString("ChatroomB_BackendContext")!;
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await sqlConnection.OpenAsync();
                    string sql = "exec ValidateRefreshToken @Token, @UserId";
                    var result = await sqlConnection.ExecuteScalarAsync<bool>(sql, new { Token = token, UserId = userId });
                    return result;
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to execute ValidateRefreshToken", ex);
                }
            }
        }

        public async Task<string> ValidateAccessToken(int userId)
        {
            string connectionString = _config.GetConnectionString("ChatroomB_BackendContext")!;
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await sqlConnection.OpenAsync();
                    string sql = "exec ValidateAccessToken @UserId";
                    string result = await sqlConnection.ExecuteScalarAsync<string>(sql, new { UserId = userId });
                    return result;
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to execute ValidateAccessToken", ex);
                }
            }
        }


        public async Task<bool> StoreRefreshToken(RefreshToken token)
        {
            try
            {
                string sql = "exec StoreRefreshToken @UserId, @Token, @ExpiredDateTime, @Success OUTPUT";

                var parameters = new DynamicParameters();
                parameters.Add("@UserId", token.UserId);
                parameters.Add("@Token", token.Token);
                parameters.Add("@ExpiredDateTime", token.ExpiredDateTime);
                parameters.Add("@Success", dbType: DbType.Boolean, direction: ParameterDirection.Output);

                await _dbConnection.ExecuteAsync(sql, parameters);

                bool isSuccess = parameters.Get<bool>("@Success");

                return isSuccess;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to execute StoreRefreshToken", ex);
            }
        }


        public async Task<bool> RemoveRefreshToken(string token)
        {
            try
            {
                string sql = "exec RemoveRefreshToken @Token, @Success OUTPUT";

                var parameters = new DynamicParameters();
                parameters.Add("@Token", token);
                parameters.Add("@Success", dbType: DbType.Boolean, direction: ParameterDirection.Output);

                await _dbConnection.ExecuteAsync(sql, parameters);

                bool isSuccess = parameters.Get<bool>("@Success");

                return isSuccess;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to execute RemoveRefreshToken", ex);
            }
        }

        public async Task<bool> UpdateRefreshToken(string token)
        {
            try
            {
                string sql = "exec UpdateRefreshTokenExpiry @Token, @ExpiredDateTime, @Success OUTPUT";
                DateTime expirationDateTime = DateTime.UtcNow.AddDays(Convert.ToInt32(_config["RefreshTokenSettings:ExpirationDays"]));

                var parameters = new DynamicParameters();
                parameters.Add("@Token", token);
                parameters.Add("@ExpiredDateTime", expirationDateTime);
                parameters.Add("@Success", dbType: DbType.Boolean, direction: ParameterDirection.Output);

                await _dbConnection.ExecuteAsync(sql, parameters);

                bool isSuccess = parameters.Get<bool>("@Success");

                return isSuccess;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to execute UpdateRefreshToken", ex);
            }
        }
    }
}
