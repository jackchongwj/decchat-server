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
                HashEntry[] hashEntries = await _redisDatabase.HashGetAllAsync(key);

                // Check if hashEntries array has at least one element
                if (hashEntries.Length > 0)
                {
                    // Return the second value in the first HashEntry
                    string secondValue = hashEntries[1].Value.ToString();
                    return secondValue;
                }
                else
                {
                    return "Hash entry not found or empty.";
                }
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }
    }
}
