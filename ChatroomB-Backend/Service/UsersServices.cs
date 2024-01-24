using ChatroomB_Backend.DTO;
using ChatroomB_Backend.Models;
using ChatroomB_Backend.Repository;
using NuGet.Protocol.Core.Types;

namespace ChatroomB_Backend.Service
{
    public class UsersService : IUserService
    {
        private readonly IUserRepo _repo;

        public UsersService(IUserRepo reponsitory)
        {
            _repo = reponsitory;
        }

        public async Task<IEnumerable<Users>> GetByName(string profileName)
        {
            return (await _repo.GetByName(profileName));
        }

        public async Task<IEnumerable<ChatlistVM>> GetChatListByUserId(int userId)
        {
            return await _repo.GetChatListByUserId(userId);
        }
    }
}
