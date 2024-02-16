using ChatroomB_Backend.DTO;
using ChatroomB_Backend.Models;
using ChatroomB_Backend.Service;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;
using System.Data;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.Threading.Tasks;


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
            var param = new DynamicParameters();
            param.Add("@RoomName", "");
            param.Add("@RoomType", 0);
            param.Add("@RoomProfilePic", "");
            param.Add("@SenderId", request.SenderId);
            param.Add("@ReceiverId", request.ReceivedId);
            param.Add("@ChatRoomId", dbType: DbType.Int32, direction: ParameterDirection.Output);

            string sql = "exec CreateChatRoomAndUserChatRoomWithPrivate @RoomName, @RoomType, @RoomProfilePic, @SenderId, @ReceiverId, @ChatRoomId OUTPUT";

            int result = await _dbConnection.ExecuteAsync(sql, param);

            int chatRoomId = param.Get<int>("@ChatRoomId");

            return chatRoomId;
        }


        public async Task <ChatlistVM?> CreateGroup(string roomName, int initiatedBy, DataTable selectedUsers)
        {
            try
            {
                var dynamicParam = new DynamicParameters();
                dynamicParam.Add("@RoomName", roomName);
                dynamicParam.Add("@InitiatedBy", initiatedBy);
                dynamicParam.Add("@SelectedUsers", selectedUsers.AsTableValuedParameter("IntListTableType"));

                ChatlistVM chatinfo = await _dbConnection.QuerySingleAsync <ChatlistVM>("CreateGroup", dynamicParam, commandType: CommandType.StoredProcedure);
                return chatinfo;

                /*() _dbConnection.Query("CreateGroup", dynamicParam, commandType: CommandType.StoredProcedure);*/
                /*                dynamicParam.Add("@SelectedUsers", selectedUserIds); // Pass the converted List<int>
                */

                /*var result = await _dbConnection.QueryFirstOrDefaultAsync<CreateGroupVM>("CreateGroup", dynamicParam, commandType: CommandType.StoredProcedure);
                return result; // Return the result of the stored procedure execution*/


                /*// Retrieve the ChatRoomId from the output parameter
                int generatedChatRoomId = dynamicParam.Get<int>("@ChatRoomId");*/
                /*
                                // Create a new instance of CreateGroupVM and set its properties
                                var result = new CreateGroupVM
                                {
                                    RoomName = roomName,
                                    ChatRoomId = generatedChatRoomId,
                                    InitiatedBy = initiatedBy,
                                    SelectedUsers = selectedUsers

                                };*/

                // Return the CreateGroupVM object
/*                return chatinfo;
*/            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                throw;
            }
        }      
    }
}

