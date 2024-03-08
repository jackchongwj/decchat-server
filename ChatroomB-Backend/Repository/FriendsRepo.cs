using Azure.Core;
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

        public async Task <IEnumerable<Users>> AddFriends(Friends friends)
        {
            try
            {
                var param = new
                {
                    SenderId = friends.SenderId,
                    ReceiverId = friends.ReceiverId,
                };
                string sql = "exec AddFriend @SenderId, @ReceiverId";

                IEnumerable<Users> result = await _dbConnection.QueryAsync<Users>(sql, param);

                return result;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to add friends", ex);
            }
        }

        public async Task<int> UpdateFriendRequest(FriendRequest request)
        {
            try
            {
                var param = new
                {
                    SenderId = request.SenderId,
                    ReceiverId = request.ReceiverId,
                    Status = request.Status
                };

                string sql = "exec UpdateFriendRequest @SenderId, @ReceiverId, @Status";

                int result = await _dbConnection.ExecuteAsync(sql, param);

                return result;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to update friend request", ex);
            }
        }

        public async Task<int> DeleteFriendRequest(int chatRoomId, int userId1, int userId2)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@ChatRoomId", chatRoomId);
                parameters.Add("@User1", userId1);
                parameters.Add("@User2", userId2);
                parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await _dbConnection.ExecuteAsync("DeleteFriend", parameters, commandType: CommandType.StoredProcedure);

                int isSuccess = parameters.Get<int>("@Result");

                return isSuccess;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to delete friend request", ex);
            }
        }

        public async Task<int> CheckFriendExit(Friends friends)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@SenderId", friends.SenderId);
                parameters.Add("@ReceiverId", friends.ReceiverId);
                parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await _dbConnection.ExecuteAsync("CheckFriendExit", parameters, commandType: CommandType.StoredProcedure);

                int isSuccess = parameters.Get<int>("@Result");

                return isSuccess;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to check friend existence", ex);
            }
        }
    }
}
