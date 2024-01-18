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

        public async Task<IEnumerable<Users>> GetByName(string profileName, int userId)
        {
            return (await _repo.GetByName(profileName, userId));
        }

        public async Task<IEnumerable<Users>> GetFriendRequest(int userId)
        {
            return (await _repo.GetFriendRequest(userId));
        }
    }
}
