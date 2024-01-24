﻿using ChatroomB_Backend.Data;
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

        public async Task<IEnumerable<UserSearch>> GetByName(string profileName, int userId)
        {
            string sql = "exec GetUserByProfileName @profileName, @userId";

            IEnumerable<UserSearch> result = await _dbConnection.QueryAsync<UserSearch>(sql, new {profileName, userId });

            return result;
        }

        public async Task<IEnumerable<Users>> GetFriendRequest(int userId)
        {
            string sql = "exec GetFriendsRequest @userId";

            IEnumerable<Users> result = await _dbConnection.QueryAsync<Users>(sql, new { userId });

            return result;
        }
    }
}
