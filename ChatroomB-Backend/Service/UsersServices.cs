using Azure.Core;
using ChatroomB_Backend.DTO;
using ChatroomB_Backend.Hubs;
using ChatroomB_Backend.Models;
using ChatroomB_Backend.Repository;
using Microsoft.AspNetCore.SignalR;
using NuGet.Protocol.Core.Types;
using System.Reflection.Metadata.Ecma335;

namespace ChatroomB_Backend.Service
{
    public class UsersServices : IUserService
    {
        private readonly IUserRepo _repo;
        private readonly IBlobService _blobService;
        private readonly IRedisServcie _RServices;
        private readonly IHubContext<ChatHub> _hubContext;

        public UsersServices(IUserRepo repository, IBlobService blobService, IRedisServcie rServices, IHubContext<ChatHub> hubContext)
        {
            _repo = repository;
            _blobService = blobService;
            _RServices = rServices;
            _hubContext = hubContext;
        }

        public async Task<IEnumerable<UserSearchDetails>> GetByName(string profileName, int userId)
        {
            return await _repo.GetByName(profileName, userId);
        }

        public async Task<IEnumerable<Users>> GetFriendRequest(int userId)
        {
            return await _repo.GetFriendRequest(userId);
        }

        public async Task<Users> GetUserById(int userId)
        {
            return await _repo.GetUserById(userId);
        }

        public async Task<int> UpdateProfileName(int userId, string newProfileName)
        {
            try
            {
                int updateResult = await _repo.UpdateProfileName(userId, newProfileName);

                if (updateResult > 0)
                {
                    IEnumerable<ChatlistVM> chatList = await GetChatListByUserId(userId);

                    List<int> friendIds = chatList.Select(chat => chat.UserId).Distinct().ToList();

                    await _hubContext.Clients.Groups(friendIds.Select(id => $"User{id}").ToList())
                        .SendAsync("ReceiveUserProfileUpdate", new { UserId = userId, ProfileName = newProfileName });
                }
                return updateResult;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return -1;
            }
        }

        public async Task<int> UpdateProfilePicture(int userId, byte[] fileBytes, string fileName)
        {
            try
            {
                // Attempt to load the image to verify if it's a valid image file
                using (Image image = SixLabors.ImageSharp.Image.Load(fileBytes))
                {
                    string blobUri = await _blobService.UploadImageFiles(fileBytes, fileName, 2);

                    int updateResult = await _repo.UpdateProfilePicture(userId, blobUri);

                    if (updateResult > 0)
                    {
                        IEnumerable<ChatlistVM> chatList = await GetChatListByUserId(userId);
                        List<int> friendIds = chatList.Select(chat => chat.UserId).Distinct().ToList();
                        await _hubContext.Clients.Groups(friendIds.Select(id => $"User{id}").ToList())
                            .SendAsync("ReceiveUserProfileUpdate", new { UserId = userId, ProfilePicture = blobUri });
                    }
                    return updateResult;
                }
            }
            catch (SixLabors.ImageSharp.UnknownImageFormatException)
            {
                Console.WriteLine("Uploaded file is not a valid image.");
                return -1;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return -1;
            }
        }


        public async Task<int> DeleteUser(int userId)
        {
            return await _repo.DeleteUser(userId);
        }

        public async Task<IEnumerable<ChatlistVM>> GetChatListByUserId(int userId)
        {
            List<string> onlineUserIds = await _RServices.GetAllUserIdsFromRedisSet();

            IEnumerable<ChatlistVM> chatlist = await _repo.GetChatListByUserId(userId);

            foreach (var chatItem in chatlist)
            {
                if (onlineUserIds.Contains(chatItem.UserId.ToString()))
                {
                    chatItem.IsOnline = true;
                }
            }
            return chatlist;
        }

        public async Task<bool> DoesUsernameExist(string username)
        {
            return await _repo.DoesUsernameExist(username);
        }

        public async Task<string> GetProfilePictureUrl(byte[] fileByte, string filename)
        {
            return await _blobService.UploadImageFiles(fileByte, filename, 2);
        }
    }
}
