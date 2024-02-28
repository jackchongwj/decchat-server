﻿using ChatroomB_Backend.Models;
using Dapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace ChatroomB_Backend.Repository
{
    public class TokenRepo : ITokenRepo

    {
        private readonly IDbConnection _dbConnection;

        public TokenRepo(IDbConnection db) 
        {
            _dbConnection = db;
        }

        public async Task<bool> IsRefreshTokenValid(RefreshToken token)
        {
            Console.WriteLine(token.Token);
            try
            {
                string sql = "exec CheckRefreshTokenValidity @Token";

                int result = await _dbConnection.ExecuteScalarAsync<int>(sql, new { Token = token.Token });

                bool isValid = Convert.ToBoolean(result);

                return isValid;
            }
            catch (SqlException ex)
            {
                throw new InvalidOperationException("A database error occurred while validating the refresh token", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An unexpected error occurred", ex);
            }
        }

        public async Task<IActionResult> StoreRefreshToken(RefreshToken token)
        {
            try
            {
                string sql = "exec StoreRefreshToken @UserId, @Token, @ExpiredDateTime";

                await _dbConnection.ExecuteAsync(sql, new RefreshToken
                {
                    UserId = token.UserId,
                    Token = token.Token,
                    ExpiredDateTime = token.ExpiredDateTime,
                });

                return new OkObjectResult(new { Message = "Refresh token stored successfully" });
            }
            catch 
            {
                return new BadRequestObjectResult(new { Error = "Failed to store refresh token" });
            }
        }

        public async Task<IActionResult> RemoveRefreshToken(RefreshToken token)
        {
            try
            {
                string sql = "exec RemoveRefreshToken @Token";

                await _dbConnection.ExecuteAsync(sql, new { token.Token });

                return new OkObjectResult(new { Message = "Refresh token removed successfully" });
            }
            catch
            {
                return new BadRequestObjectResult(new { Error = "Failed to remove refresh token" });
            }
        }

    }
}
