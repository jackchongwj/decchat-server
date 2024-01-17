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
            string sql = "exec GetUserByProfileName @profileName";

            IEnumerable<Users> result = await _dbConnection.QueryAsync<Users>(sql, new {profileName});

            return result;
        }
    }
}
