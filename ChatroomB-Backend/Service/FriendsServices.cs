using ChatroomB_Backend.Models;
using ChatroomB_Backend.Repository;

namespace ChatroomB_Backend.Service
{
    public class FriendsServices : IFriendService
    {
        private readonly IFriendRepo _repo;

        public FriendsServices(IFriendRepo _repository) 
        {
            _repo = _repository;
        }

        public async Task<int> AddFriends(Friends friends)
        {
            return (await _repo.AddFriends(friends));
        }

        public async Task<IEnumerable<Users>> GetFriendList(int userId)
        {
            return await _repo.GetFriendList(userId);
        }

    }
}
