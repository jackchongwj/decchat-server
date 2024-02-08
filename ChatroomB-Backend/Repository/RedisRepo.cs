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

        public async Task<int> AddUserIdAndConnetionIdToRedis(string userId, string connectionId)
        {
            try
            {
                string key = $"User:{userId}:connection";

                // set hash
                await _redisDatabase.HashSetAsync(key, new[] 
                {
                    new HashEntry("UserId", userId),
                    new HashEntry("ConnectionId", connectionId)
                });

                //set time
                //await _redisDatabase.KeyExpireAsync(key, TimeSpan.FromDays(7));

                return 1;

            } catch (Exception ex)
            {
                return 0;
            }
        }

        public async Task<int> DeleteUserIdFromRedis(string userId)
        {
            try
            {
                string key = $"User:{userId}:connection";

                // set hash
                bool deleted = await _redisDatabase.KeyDeleteAsync(key);

                if (deleted)
                {
                    Console.WriteLine($"Key '{deleted}' deleted successfully.");
                    return 1;
                }
                else
                {
                    Console.WriteLine($"Key '{deleted}' not found or couldn't be deleted.");
                    return 0;
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public async Task<string> SelectUserIdFromRedis(int? userId)
        {
            try
            {
                string key = $"User:{userId}:connection";

                // get hash
                RedisValue data = await _redisDatabase.StringGetAsync(key);

                string result = data.ToString();

                return result;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
