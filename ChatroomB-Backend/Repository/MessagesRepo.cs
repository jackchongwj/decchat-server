using ChatroomB_Backend.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using NuGet.Packaging.Signing;
using System.Data;

namespace ChatroomB_Backend.Repository
{
    public class MessagesRepo : IMessageRepo
    {
        private readonly IDbConnection _dbConnection;
        private readonly string _dbConnectionString;

        public MessagesRepo(IDbConnection db)
        {
            _dbConnection = db;
            _dbConnectionString = db.ConnectionString;
        }

        public async Task<int> AddMessages(Messages message)
        {
            var param = new
            {
                Content = message.Content,
                UserChatRoomId = message.UserChatRoomId,
                TimeStamp = message.TimeStamp,
                ResourceUrl = message.ResourceUrl,
                MessageType = message.MessageType,
                IsDeleted = message.IsDeleted,
            };

            using (SqlConnection connection = new SqlConnection(_dbConnectionString))
            {
                string StoredProcedure = "AddMessage";

                int result = await connection.ExecuteAsync(StoredProcedure, param, commandType: System.Data.CommandType.StoredProcedure);
                return result;
            }
        }
    }
}
