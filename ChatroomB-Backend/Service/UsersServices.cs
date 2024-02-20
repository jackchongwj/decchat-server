﻿using ChatroomB_Backend.DTO;
using ChatroomB_Backend.Hubs;
using ChatroomB_Backend.Models;
using ChatroomB_Backend.Repository;
using Microsoft.AspNetCore.SignalR;
using NuGet.Protocol.Core.Types;

namespace ChatroomB_Backend.Service
{
    public class UsersServices : IUserService
    {
        private readonly IUserRepo _repo;
        private readonly IBlobService _blobService;
        private readonly IRedisServcie _RServices;
        private readonly IHubContext<ChatHub> _hubContext;

        public UsersServices(IUserRepo reponsitory, IBlobService blobService, IRedisServcie rServices, IHubContext<ChatHub> hubContext)
        {
            _repo = reponsitory;
            _blobService = blobService;
            _RServices = rServices;
            _hubContext = hubContext;
        }

        public async Task<IEnumerable<UserSearchDetails>> GetByName(string profileName, int userId)
        {
            return (await _repo.GetByName(profileName, userId));
        }

        public async Task<IEnumerable<Users>> GetFriendRequest(int userId)
        {
            return (await _repo.GetFriendRequest(userId));
        }

        public async Task<Users> GetUserById(int userId)
        {
            return await _repo.GetUserById(userId);
        }

        public async Task<int> UpdateProfileName(int userId, string newProfileName)
        {
            return await _repo.UpdateProfileName(userId, newProfileName);
        }

        public async Task<bool> UpdateProfilePicture(int userId, byte[] fileBytes, string fileName)
        {
            try
            {
                // Upload the file to blob storage and get the URI
                string blobUri = await _blobService.UploadImageFiles(fileBytes, fileName, 2);

                // Update the user's profile picture URI in the database
                int updateResult = await _repo.UpdateProfilePicture(userId, blobUri);

                // Assuming the updateResult is an int that signifies the number of records updated
                // You might want to check if it actually succeeded based on your repository implementation
                return updateResult != 0;
            }
            catch (Exception ex)
            {
                // Depending on your logging framework, log the exception
                Console.WriteLine($"An error occurred: {ex.Message}");
                return false;
            }
        }

        public async Task<int> DeleteUser(int userId)
        {
            return await _repo.DeleteUserProfile(userId);
        }

        public async Task<IEnumerable<ChatlistVM>> GetChatListByUserId(int userId)
        {
            IEnumerable<ChatlistVM> chatlist = await _repo.GetChatListByUserId(userId);

            if (chatlist != null)
            {
                // add chalist to signalR group for send message
                string connectionId = await _RServices.SelectUserIdFromRedis(userId);

                foreach (var list in chatlist) 
                {
                    await _hubContext.Groups.AddToGroupAsync(connectionId, list.ChatRoomId.ToString());

                    Console.WriteLine($"{connectionId} has joined the group {list.ChatRoomId}");
                }
                  
            }
            return chatlist; 
        }

        public async Task<bool> DoesUsernameExist(string username)
        {
            return await _repo.DoesUsernameExist(username);
        }

        public async Task<int> GetUserId(string username)
        {
            return await _repo.GetUserId(username);
        }

        public async Task<string> GetUserName(int userId)
        {
            return await _repo.GetUserName(userId);
        }

        public async Task<string> GetProfilePictureUrl(byte[] fileByte, string filename)
        {
            return await _blobService.UploadImageFiles(fileByte, filename, 2);
        }
    }
}
