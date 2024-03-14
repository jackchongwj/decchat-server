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

        public async Task <ChatRoomMessage> AddMessages(ChatRoomMessage message)
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

                    return result;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to execute AddMessages", ex);
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
                throw new Exception("Failed to execute DeleteMessage", ex);
            }
        }

        public async Task<int> EditMessage(EditMessage NewMessage)
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
                throw new Exception("Failed to execute EditMessage", ex);
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
                throw new Exception("Failed to execute GetMessages", ex);
            }
        }

        public async Task<int> GetTotalSearchMessage(int ChatRoomId, string SearchValue)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@ChatRoomId", ChatRoomId);
                parameters.Add("@SearchValue", SearchValue);
                parameters.Add("@RowCount", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await _dbConnection.ExecuteAsync("GetTotalSearchMessage", parameters, commandType: CommandType.StoredProcedure);

                int count = parameters.Get<int>("@RowCount");

                return count;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to execute GetTotalSearchMessage", ex);
            }
        }
    }
}
