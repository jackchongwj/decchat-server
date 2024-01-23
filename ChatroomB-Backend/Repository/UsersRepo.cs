using ChatroomB_Backend.Data;
using ChatroomB_Backend.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Dapper;
using System.Data;
using System.Collections.Generic;

namespace ChatroomB_Backend.Repository
{
    public class UsersRepo : IUserRepo
    {

        private readonly IDbConnection _dbConnection;

        public UsersRepo(IDbConnection db)
        {
            _dbConnection = db;
        }

        public async Task<IEnumerable<Users>> GetByName(string profileName)
        {
            try
            {
                string sql = "exec GetUserByProfileName @profileName";

                IEnumerable<Users> result = await _dbConnection.QueryAsync<Users>(sql, new { profileName });

                return result;
            }
            catch
            {
                throw new Exception("An unexpected error occurred");
            }
        }

        public async Task<bool> IsUsernameUnique(string username)
        {
            try
            {
                string sql = "exec CheckUserNameUnique @UserName";

                bool isUnique = await _dbConnection.ExecuteScalarAsync<bool>(sql, new { UserName = username });

                return isUnique;
            }
            catch
            {
                throw new Exception("An unexpected error occurred");
            }
        }

        public async Task<int> GetUserId(string username)
        {
            try
            {
                string sql = "exec GetUserIdByUserName @username";

                int result = await _dbConnection.ExecuteScalarAsync<int>(sql, new { UserName = username });

                return result;
            }
            catch
            {
                throw new Exception("An unexpected error occurred");
            }
            
        }
    }
}
