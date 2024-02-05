using ChatroomB_Backend.DTO;
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

        public UsersServices(IUserRepo reponsitory)
        {
            _repo = reponsitory;
        }

        public async Task<IEnumerable<UserSearch>> GetByName(string profileName, int userId)
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

        public async Task<int> UpdateProfilePicture(int userId, string newProfilePicture)
        {
            return await _repo.UpdateProfilePicture(userId, newProfilePicture);
        }

        public async Task<string> UploadProfilePicture(IFormFile file, int userId)
        {
            // Convert IFormFile to byte[] for the BlobService if necessary
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                var fileBytes = memoryStream.ToArray();

                // Assuming the filename is important for blob storage
                var blobUri = await _blobService.UploadImageFiles(fileBytes, file.FileName, 2); // 2 for User Profile Picture
                return blobUri;
            }
        }

        public async Task<int> DeleteUser(int userId)
        {
            return await _repo.DeleteUserProfile(userId);
        }

        public async Task<IEnumerable<ChatlistVM>> GetChatListByUserId(int userId)
        {
            return await _repo.GetChatListByUserId(userId);
        }

        public async Task<bool> DoesUsernameExist(string username)
        {
            return await _repo.DoesUsernameExist(username);
        }

        public async Task<int> GetUserId(string username)
        {
            return await _repo.GetUserId(username);
        }
    }
}
