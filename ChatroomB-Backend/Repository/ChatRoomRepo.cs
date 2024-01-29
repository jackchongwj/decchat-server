using ChatroomB_Backend.DTO;
using ChatroomB_Backend.Models;
using ChatroomB_Backend.Service;
using Dapper;
using NuGet.Protocol.Plugins;
using System.Data;

namespace ChatroomB_Backend.Repository
{
    public class ChatRoomRepo : IChatRoomRepo
    {
        private readonly IDbConnection _dbConnection;

        public ChatRoomRepo(IDbConnection db)
        {
            _dbConnection = db;
        }

        public async Task<int> AddChatRoom(FriendRequest request)
        {
            var param = new
            {
                RoomName = "",
                RoomType = 0,
                RoomProfilePic = "",
                InitiatedBy = request.SenderId,
                SenderId = request.SenderId,
                ReceiverId = request.ReceivedId

            };
            string sql = "exec CreateChatRoomAndUserChatRoomWithPrivate @RoomName, @RoomType, @RoomProfilePic, @SenderId, @ReceiverId ";

            int result = await _dbConnection.ExecuteAsync(sql, param);

            return result;
        }
    }
}
