using ChatroomB_Backend.Data;
using ChatroomB_Backend.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Dapper;
using System.Data;
using System.Collections.Generic;
using ChatroomB_Backend.DTO;

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
        public async Task<IEnumerable<ChatlistVM>> GetChatListByUserId(int userId)
        {
            var parameter = new DynamicParameters();
            parameter.Add("@UserId", userId);

            string sql = "EXEC GetChatListByUserId @UserId";

           var chatList = await _dbConnection.QueryAsync<ChatlistVM>(sql, parameter);

            return chatList.AsList();
        }


    }
}
