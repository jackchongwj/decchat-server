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

        public async Task<ActionResult> StoreRefreshToken(RefreshToken token)
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

        public async Task<ActionResult> RemoveRefreshToken(RefreshToken token)
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

        public async Task<ActionResult> ValidateRefreshToken(RefreshToken token)
        {
            try
            {
                string sql = "exec ValidateRefreshToken @Token";

                RefreshToken refreshToken = await _dbConnection.QueryFirstOrDefaultAsync<RefreshToken>(sql, new { token.Token });

                if (refreshToken == null)
                {
                    return new BadRequestObjectResult(new { Error = "Invalid refresh token" });
                }

                if (refreshToken.ExpiredDateTime < DateTime.UtcNow)
                {
                    return new BadRequestObjectResult(new { Error = "Refresh token has expired" });
                }

                return new OkObjectResult(new { Message = "Refresh token is valid", RefreshToken = refreshToken });
            }
            catch
            {
                return new BadRequestObjectResult(new { Error = "Failed to validate refresh token" });
            }
        }
    }
}
