using ChatroomB_Backend.Models;
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
            string sql = "exec CheckRefreshTokenValidity @Token";
            int result = await _dbConnection.ExecuteScalarAsync<int>(sql, new { Token = token.Token });
            bool isValid = Convert.ToBoolean(result);

            return isValid;
        }

        public async Task<IActionResult> StoreRefreshToken(RefreshToken token)
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

        public async Task<IActionResult> RemoveRefreshToken(RefreshToken token)
        {
            string sql = "exec RemoveRefreshToken @Token";
            await _dbConnection.ExecuteAsync(sql, new { token.Token });

            return new OkObjectResult(new { Message = "Refresh token removed successfully" });
        }
    }
}
