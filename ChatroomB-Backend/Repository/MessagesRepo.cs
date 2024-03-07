using ChatroomB_Backend.DTO;
using ChatroomB_Backend.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
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

        public async Task <ChatRoomMessage> AddMessages(Messages message)
        {
            var param = new
            {
                Content = message.Content,
                UserChatRoomId = message.UserChatRoomId,
                TimeStamp = message.TimeStamp,
                ResourceUrl = message.ResourceUrl,
                IsDeleted = message.IsDeleted,
            };

            try
            {
                using (SqlConnection connection = new SqlConnection(_dbConnectionString))
                {
                    await connection.OpenAsync(); // Ensure the connection is open
                    string StoredProcedure = "AddMessage";

                    ChatRoomMessage result = await connection.QueryFirstAsync<ChatRoomMessage>(StoredProcedure, param, commandType: System.Data.CommandType.StoredProcedure);

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

        public async Task<int> DeleteMessage(int MessageId)
        {
            string sql = "exec DeleteMessage @MessageId";

            return await _dbConnection.ExecuteAsync(sql, new { MessageId = MessageId });
        }

        public async Task<int> EditMessage(ChatRoomMessage NewMessage)
        {
            try
            {
                var param = new
                {
                    MessageId = NewMessage.MessageId,
                    Content = NewMessage.Content
                };

                using (SqlConnection connection = new SqlConnection(_dbConnectionString))
                {
                    await connection.OpenAsync(); // Ensure the connection is open
                    string StoredProcedure = "EditMessage";

                    int result = await connection.QueryFirstOrDefaultAsync<int>(StoredProcedure, param, commandType: System.Data.CommandType.StoredProcedure);
                    return result;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return 0;
        }

        public async Task<IEnumerable<ChatRoomMessage>> GetMessages(int ChatRoomId, int MessageId)
        {
            var param = new
            {
                ChatRoomId = ChatRoomId,
                MessageId = MessageId
            };

            string sql = "exec RetrieveMessageByPagination @ChatRoomId, @MessageId ";

            IEnumerable<ChatRoomMessage> result = await _dbConnection.QueryAsync<ChatRoomMessage>(sql, param);

            return result;
        }
    }
}
