using System.Threading.Tasks;
using StackExchange.Redis;

namespace FoodService.Services
{
    public class RedisService : ICacheService
    {
        private readonly IConnectionMultiplexer _redisConnection;

        public RedisService(IConnectionMultiplexer connection)
        {
            _redisConnection = connection;
        }

        public async Task<HashEntry[]> GetHashAsync(string key)
        {
            var db = _redisConnection.GetDatabase();
            return  await db.HashGetAllAsync(key);
        }
    }
}