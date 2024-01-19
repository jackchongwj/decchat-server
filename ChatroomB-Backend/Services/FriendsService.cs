using ChatroomB_Backend.Models;
using ChatroomB_Backend.Repository;

namespace ChatroomB_Backend.Service
{
    public class FriendsService : IFriendService
    {
        private readonly IFriendRepo _repo;

        public FriendsService(IFriendRepo _repository) 
        {
            _repo = _repository;
        }

        public async Task<int> AddFriends(Friends friends)
        {
            return (await _repo.AddFriends(friends));
        }
    }
}
