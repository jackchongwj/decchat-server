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

        public async Task<int> AddUserIdAndConnetionIdToRedis(string userId, string connectionId)
        {
            return await _repo.AddUserIdAndConnetionIdToRedis(userId, connectionId);
        }

        public async Task<int> DeleteUserIdFromRedis(string userId)
        {
            return await _repo.DeleteUserIdFromRedis(userId);
        }

        public async Task<string> SelectUserIdFromRedis(int? userId)
        {
            return await _repo.SelectUserIdFromRedis(userId);
        }

        public async Task<List<string>> GetAllUserIdsFromRedisSet()
        {
            return await _repo.GetAllUserIdsFromRedisSet();
        }
    }
}
