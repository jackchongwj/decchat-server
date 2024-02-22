using ChatroomB_Backend.DTO;
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
        private readonly IHubContext<ChatHub> _hubContext;

        public UsersServices(IUserRepo reponsitory, IBlobService blobService, IHubContext<ChatHub> hubContext)
        {
            _repo = reponsitory;
            _blobService = blobService;
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
            var updateResult = await _repo.UpdateProfileName(userId, newProfileName);
            if (updateResult > 0 )
            {
                var chatList = await GetChatListByUserId(userId);
                List<int> friendIds = chatList.Select(chat => chat.UserId).Distinct().ToList(); // Use UserId as friend ID
                await _hubContext.Clients.Groups(friendIds.Select(id => $"FR{id}").ToList()).SendAsync("ReceiveUserProfileUpdate", new { UserId = userId, ProfileName = newProfileName });
            }
            return updateResult;
        }

        public async Task<int> UpdateProfilePicture(int userId, byte[] fileBytes, string fileName)
        {
            try
            {
                // Upload the file to blob storage and get the URI
                string blobUri = await _blobService.UploadImageFiles(fileBytes, fileName, 2);

                // Update the user's profile picture URI in the database
                int updateResult = await _repo.UpdateProfilePicture(userId, blobUri);

                // If the profile picture was successfully updated
                if (updateResult > 0)
                {
                    // Fetch the list of chatrooms that includes the user's friends
                    var chatList = await GetChatListByUserId(userId);

                    // Extract friend IDs from the chat list
                    List<int> friendIds = chatList.Select(chat => chat.UserId).Distinct().ToList();

                    // Broadcast the profile picture update to all friends
                    await _hubContext.Clients.Groups(friendIds.Select(id => $"FR{id}").ToList())
                        .SendAsync("ReceiveUserProfileUpdate", new { UserId = userId, ProfilePicture = blobUri });
                }

                return updateResult;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                // Return a value indicating failure, such as -1, to differentiate from successful updates
                return -1;
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

        public async Task<string> GetProfilePictureUrl(byte[] fileByte, string filename)
        {
            return await _blobService.UploadImageFiles(fileByte, filename, 2);
        }
    }
}
