using ChatroomB_Backend.DTO;
using ChatroomB_Backend.Models;
using ChatroomB_Backend.Repository;
using ChatroomB_Backend.SignalR;
using Microsoft.AspNetCore.SignalR;
using NuGet.Protocol.Core.Types;

namespace ChatroomB_Backend.Service
{
    public class UsersService : IUserService
    {
        private readonly IUserRepo _repo;
        private readonly IHubContext<ChatHub> _hub;

        public UsersService(IUserRepo reponsitory, IHubContext<ChatHub> _hubContext)
        {
            _repo = reponsitory;
            _hub = _hubContext;
        }

        public async Task<IEnumerable<UserSearch>> GetByName(string profileName, int userId)
        {
            return (await _repo.GetByName(profileName, userId));
        }

        public async Task<IEnumerable<Users>> GetFriendRequest(int userId)
        {
            return (await _repo.GetFriendRequest(userId));
        }
    }
}
