using ChatroomB_Backend.Models;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace ChatroomB_Backend.Repository
{
    public class RedisRepo : IRedisRepo
    {
        private readonly IDatabase _redisDatabase;

        public RedisRepo(IConnectionMultiplexer redisConnectionMultiplexer)
        {
            _redisDatabase = redisConnectionMultiplexer.GetDatabase();
        }

        public async Task<int> AddPrivateChatRoomToRedis(Friends friends)
        {
            try
            {
                string key = "friend:" + friends.RequestId;

                // set JSON key type
                await _redisDatabase.ExecuteAsync("JSON.SET", key, ".", JsonConvert.SerializeObject(friends));

                //set time
                await _redisDatabase.KeyExpireAsync(key, TimeSpan.FromDays(7));

                return 1;
            } catch (Exception ex)
            {

                return 0;
            }
        }

    }
}
