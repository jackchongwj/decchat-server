using ChatroomB_Backend.Models;
using ChatroomB_Backend.Repository;
using NuGet.Protocol.Core.Types;

namespace ChatroomB_Backend.Service
{
    public class UsersServices : IUserService
    {
        private readonly IUserRepo _repo;

        public UsersServices(IUserRepo reponsitory)
        {
            _repo = reponsitory;
        }

        public async Task<IEnumerable<Users>> GetByName(string profileName)
        {
            return (await _repo.GetByName(profileName));
        }

        public async Task<bool> IsUsernameUnique(string username)
        {
            return await _repo.IsUsernameUnique(username);
        }

        public async Task<int> GetUserId(string username)
        {
            return await _repo.GetUserId(username);
        }
    }
}
