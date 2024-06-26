﻿using ChatroomB_Backend.DTO;
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
using System.Data.Common;
using System.Security.Cryptography;


namespace ChatroomB_Backend.Repository
{
    public class ChatRoomRepo : IChatRoomRepo
    {
        private readonly IDbConnection _dbConnection;
        private readonly IConfiguration _config;

        public ChatRoomRepo(IDbConnection db, IConfiguration config)
        {
            _dbConnection = db;
            _config = config;
        }

        public async Task<IEnumerable<ChatlistVM>> AddChatRoom(FriendRequest request)
        {
            try
            {
                var param = new
                {
                    RoomName = "",
                    RoomType = 0,
                    RoomProfilePic = "",
                    SenderId = request.SenderId,
                    ReceiverId = request.ReceiverId,
                    UserId = request.ReceiverId  // Adjust the parameter name
                };

                string sql = "exec CreateChatRoomAndUserChatRoomWithPrivate @RoomName, @RoomType, @RoomProfilePic, @SenderId, @ReceiverId, @UserId";

                IEnumerable<ChatlistVM> chatList = await _dbConnection.QueryAsync<ChatlistVM>(sql, param);

                return chatList;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to execute AddChatRoom", ex);
            }
        }

        public async Task<int> UpdateGroupName(int chatRoomId, string newGroupName)
        {
            try
            {
                string sql = "exec UpdateGroupName @ChatRoomId, @NewGroupName";

                int result = await _dbConnection.ExecuteAsync(sql, new
                {
                    chatRoomId,
                    newGroupName
                });

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to execute UpdateGroupName", ex);
            }
        }

        public async Task<int> UpdateGroupPicture(int chatRoomId, string newGroupPicture)
        {
            try
            {
                string sql = "exec UpdateGroupPicture @ChatRoomId, @NewGroupPicture";

                int result = await _dbConnection.ExecuteAsync(sql, new
                {
                    chatRoomId,
                    newGroupPicture
                });

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to execute UpdateGroupPicture", ex);
            }
            
        }

        public async Task<IEnumerable<ChatlistVM>> CreateGroup(string roomName, int initiatedBy, DataTable selectedUsers)
        {
            try
            {
                var dynamicParam = new DynamicParameters();
                dynamicParam.Add("@RoomName", roomName);
                dynamicParam.Add("@RoomProfilePic", _config["DefaultPicture:GroupProfile"]);
                dynamicParam.Add("@InitiatedBy", initiatedBy);
                dynamicParam.Add("@SelectedUsers", selectedUsers.AsTableValuedParameter("IntListTableType"));

                IEnumerable<ChatlistVM> chatinfo = await _dbConnection.QueryAsync<ChatlistVM>("CreateGroup", dynamicParam, commandType: CommandType.StoredProcedure);

                return chatinfo;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to execute CreateGroup", ex);
            }
        }

        public async Task<IEnumerable<GroupMember>> RetrieveGroupMemberByChatroomId(int chatRoomId, int userId)
        {
            try
            {
                string sql = "RetrieveGroupMemberByChatroomId";

                var parameters = new { ChatRoomID = chatRoomId, userId = userId };

                return await _dbConnection.QueryAsync<GroupMember>(sql, parameters);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to execute RetrieveGroupMemberByChatroomId", ex);
            }
        }

        public async Task<IEnumerable<ChatlistVM>> AddMembersToGroup(int chatRoomId, DataTable selectedUsers)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@ChatRoomID", chatRoomId);
                parameters.Add("@SelectedUsers", selectedUsers.AsTableValuedParameter("IntListTableType"));

                IEnumerable<ChatlistVM> chatinfo = await _dbConnection.QueryAsync<ChatlistVM>("AddMembersToGroup", parameters, commandType: CommandType.StoredProcedure);

                return chatinfo;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to execute AddMembersToGroup", ex);
            }
        }
        public async Task<IEnumerable<ChatlistVM>> GetGroupInfoByChatroomId(int chatRoomId, DataTable selectedUsers)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@ChatRoomID", chatRoomId);
                parameters.Add("@SelectedUsers", selectedUsers.AsTableValuedParameter("IntListTableType"));

                IEnumerable<ChatlistVM> chatList = await _dbConnection.QueryAsync<ChatlistVM>("RetrieveChatRoomInfoByChatRoomId", parameters, commandType: CommandType.StoredProcedure);

                return chatList;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to execute GetGroupInfoByChatroomId", ex);
            }
        }

        public async Task<int> RemoveUserFromGroup(int chatRoomId, int removedUserId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@ChatRoomID", chatRoomId);
                parameters.Add("@UserID", removedUserId);
                parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await _dbConnection.ExecuteAsync("RemoveUserFromGroup", parameters, commandType: CommandType.StoredProcedure);

                int isSuccess = parameters.Get<int>("@Result");

                return isSuccess;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to execute RemoveUserFromGroup", ex);
            }
        }

        public async Task<int> QuitGroup(int chatRoomId, int userId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@ChatRoomID", chatRoomId);
                parameters.Add("@UserID", userId);
                parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await _dbConnection.ExecuteAsync("QuitGroup", parameters, commandType: CommandType.StoredProcedure);
                int isSuccess = parameters.Get<int>("@Result");
                return isSuccess;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to execute QuitGroup", ex);
            }
        }
    }
}

