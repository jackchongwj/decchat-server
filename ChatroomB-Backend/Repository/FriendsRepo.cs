using ChatroomB_Backend.DTO;
using ChatroomB_Backend.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
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
                ReceiverId = friends.ReceiverId,
            };
            string sql = "exec AddFriend @SenderId, @ReceiverId";

           int result =  await _dbConnection.ExecuteAsync(sql, param);

            return result;
        }


        public async Task<int> UpdateFriendRequest(FriendRequest request)
        {
            var param = new
            {
                SenderId = request.SenderId,
                ReceiverId = request.ReceivedId,
                Status = request.Status
            };

            string sql = "exec UpdateFriendRequest @SenderId, @ReceiverId, @Status";
            
            int result = await _dbConnection.ExecuteAsync(sql, param);

            return result;

        }
    }
}
