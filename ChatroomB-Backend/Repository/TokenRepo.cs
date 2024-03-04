﻿using ChatroomB_Backend.Models;
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

        public async Task<bool> IsRefreshTokenValid(RefreshToken token, int userId, string username)
        {
            string sql = "exec CheckRefreshTokenValidity @Token, @UserId, @UserName";

            try
            {
                int result = await _dbConnection.ExecuteScalarAsync<int>(sql, new
                {
                    Token = token.Token,
                    UserId = userId,
                    UserName = username
                });
                bool isValid = Convert.ToBoolean(result);

                return isValid;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to validate refresh token", ex);
            }
        }

        public async Task<IActionResult> StoreRefreshToken(RefreshToken token)
        {
            string sql = "exec StoreRefreshToken @UserId, @Token, @ExpiredDateTime";
            int expirationDays = _config.GetValue<int>("RefreshTokenSettings:ExpirationDays");
            DateTime expirationDateTime = DateTime.UtcNow.AddDays(expirationDays);

            try
            {
                await _dbConnection.ExecuteAsync(sql, new RefreshToken
                {
                    UserId = token.UserId,
                    Token = token.Token,
                    ExpiredDateTime = expirationDateTime,
                });

                return new OkObjectResult(new { Message = "Refresh token stored successfully" });
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to store refresh token", ex);
            }
        }

        public async Task<IActionResult> RemoveRefreshToken(RefreshToken token)
        {
            string sql = "exec RemoveRefreshToken @Token";

            try
            {
                await _dbConnection.ExecuteAsync(sql, new { token.Token });

                return new OkObjectResult(new { Message = "Refresh token removed successfully" });
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to remove refresh token", ex);
            }
            
        }

        public async Task<IActionResult> UpdateRefreshToken(RefreshToken token)
        {
            string sql = "exec UpdateRefreshTokenExpiry @Token, @ExpiredDateTime";
            int expirationDays = _config.GetValue<int>("RefreshTokenSettings:ExpirationDays");
            DateTime expirationDateTime = DateTime.UtcNow.AddDays(expirationDays);
            
            try
            {
                await _dbConnection.ExecuteAsync(sql, new RefreshToken
                {
                    Token = token.Token,
                    ExpiredDateTime = expirationDateTime,
                });

                return new OkObjectResult(token);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to update refresh token", ex);
            }
        }
    }
}
