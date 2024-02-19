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
using System.Reflection.Metadata;


namespace ChatroomB_Backend.Repository
{
    public class ChatRoomRepo : IChatRoomRepo
    {
        private readonly IDbConnection _dbConnection;

        public ChatRoomRepo(IDbConnection db)
        {
            _dbConnection = db;
        }

        public async Task<IEnumerable<ChatlistVM>> AddChatRoom(FriendRequest request, int userId)
        {
            var param = new
            {
                RoomName = "",
                RoomType = 0,
                RoomProfilePic = "",
                SenderId = request.SenderId,
                ReceiverId = request.ReceiverId,
                UserId = userId  // Adjust the parameter name
            };

            string sql = "exec CreateChatRoomAndUserChatRoomWithPrivate @RoomName, @RoomType, @RoomProfilePic, @SenderId, @ReceiverId, @UserId";

            IEnumerable<ChatlistVM> chatList = await _dbConnection.QueryAsync<ChatlistVM>(sql, param);

            return chatList;
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
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                throw;
            }
        }

        public async Task<int> UpdateGroupPicture(int ChatRoomId, string newGroupPicture)
        {
            string sql = "exec UpdateGroupProfilePicture @ChatRoomId, @NewGroupProfilePic";
            int result = await _dbConnection.ExecuteAsync(sql, new
            {
                ChatRoomId,
                newGroupPicture
            });
            return result;
        }
    }
}

