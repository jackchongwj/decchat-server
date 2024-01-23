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
    }
}
