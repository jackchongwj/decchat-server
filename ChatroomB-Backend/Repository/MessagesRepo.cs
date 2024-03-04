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

        public async Task<ChatRoomMessage> AddMessages(Messages message)
        {
            try
            {
                var param = new
                {
                    Content = message.Content,
                    UserChatRoomId = message.UserChatRoomId,
                    TimeStamp = message.TimeStamp,
                    ResourceUrl = message.ResourceUrl,
                    IsDeleted = message.IsDeleted,
                };

                using (SqlConnection connection = new SqlConnection(_dbConnectionString))
                {
                    await connection.OpenAsync(); // Ensure the connection is open
                    string StoredProcedure = "AddMessage";

                    ChatRoomMessage result = await connection.QueryFirstAsync<ChatRoomMessage>(StoredProcedure, param, commandType: System.Data.CommandType.StoredProcedure);

                    Console.WriteLine($"Message  {result}");
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to add messages", ex);
            }

        }

        public async Task<int> DeleteMessage(int MessageId)
        {
            try
            {
                string sql = "exec DeleteMessage @MessageId";

                return await _dbConnection.ExecuteAsync(sql, new { MessageId = MessageId });
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to delete message", ex);
            }
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
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to edit message", ex);
            }
        }

        public async Task<IEnumerable<ChatRoomMessage>> GetMessages(int ChatRoomId, int MessageId)
        {
            try
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
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to retrieve messages", ex);
            }
        }
    }
}
