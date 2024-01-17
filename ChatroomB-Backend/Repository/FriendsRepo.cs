﻿using ChatroomB_Backend.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;
using System.Data;

namespace ChatroomB_Backend.Repository
{
    public class FriendsRepo : IFriendRepo
    {
        private readonly IDbConnection _dbConnection;

        public FriendsRepo(IDbConnection db)
        {
            _dbConnection = db;
        }

        public async Task<int> AddFriends(Friends friends)
        {
            var param = new
            {
                SenderId = friends.SenderId,
                ReceiverId = friends.ReceiverId
            };
            string sql = "exec AddFriend @SenderId, @ReceiverId";

            var result = await _dbConnection.ExecuteAsync(sql, param);

            return result;
        }

        public async Task<IEnumerable<Users>> GetFriendList(int userId)
        {
            var parameter = new DynamicParameters();
            parameter.Add("@UserId", userId);

            string sql = "EXEC GetFriendList @UserId";

            var chatList = await _dbConnection.QueryAsync<Users>(sql, parameter);

            return chatList.AsList();
        }

    }
}
