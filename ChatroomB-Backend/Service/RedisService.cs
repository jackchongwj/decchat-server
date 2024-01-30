using ChatroomB_Backend.Models;
using ChatroomB_Backend.Repository;

namespace ChatroomB_Backend.Service
{
    public class RedisService : IRedisServcie
    {
        private readonly IRedisRepo _repo;

        public RedisService(IRedisRepo _repository)
        {
            _repo = _repository;
        }

        public async Task<int> AddPrivateChatRoomToRedis(Friends friends)
        {
            return await _repo.AddPrivateChatRoomToRedis(friends);
        }
    }
}
