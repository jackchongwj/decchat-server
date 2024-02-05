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

        public UsersServices(IUserRepo reponsitory, IBlobService blobService)
        {
            _repo = reponsitory;
            _blobService = blobService;
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

        public async Task<int> UpdateUser(Users user)
        {
            return await _repo.UpdateUserProfile(user);
        }

        public async Task<int> DeleteUser(int userId)
        {
            return await _repo.DeleteUserProfile(userId);
        }

        public async Task<int> ChangePassword(int userId, string newPassword)
        {
            // Ensure newPassword is hashed appropriately before sending it to the repository
            return await _repo.ChangePassword(userId, newPassword);
        }

        public async Task<IEnumerable<ChatlistVM>> GetChatListByUserId(int userId)
        {
            return await _repo.GetChatListByUserId(userId);
        }

        public async Task<bool> IsUsernameUnique(string username)
        {
            return await _repo.IsUsernameUnique(username);
        }

        public async Task<int> GetUserId(string username)
        {
            return await _repo.GetUserId(username);
        }

        public async Task<string> GetProfilePictureUrl(byte[] fileByte, string filename)
        {
            return await _blobService.UploadImageFiles(fileByte, filename, 2);
        }
    }
}
