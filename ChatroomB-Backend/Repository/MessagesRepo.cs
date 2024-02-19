using ChatroomB_Backend.DTO;
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
        private readonly ILogger<MessagesRepo> _logger;

        public MessagesRepo(IDbConnection db, ILogger<MessagesRepo> logger)
        {
            _dbConnection = db;
            _dbConnectionString = db.ConnectionString;
            _logger = logger;
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

            try
            {
                using (SqlConnection connection = new SqlConnection(_dbConnectionString))
                {
                    await connection.OpenAsync(); // Ensure the connection is open
                    string StoredProcedure = "AddMessage";

                    int result = await connection.ExecuteAsync(StoredProcedure, param, commandType: System.Data.CommandType.StoredProcedure);
                    return result;
                }
            }
            catch (Microsoft.Data.SqlClient.SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "An error occurred when calling AddMessages.");
                throw;
            }
            catch (Exception ex)
            {
                // Handle other non-SQL exceptions
                _logger.LogError(ex, "An unexpected error occurred in AddMessages.");
                throw;
            }

        }

        public async Task<IEnumerable<ChatRoomMessage>> GetMessages(int ChatRoomId)
        {
            string sql = "exec RetrieveMessage @ChatRoomId";

            IEnumerable<ChatRoomMessage> result = await _dbConnection.QueryAsync<ChatRoomMessage>(sql, new { ChatRoomId });

            return result;
        }
    }
}
