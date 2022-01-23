using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClipX.Infrastructure.Repositories
{
    public class RedisRepository : IRedisRepository
    {
        private readonly IDatabase _db;

        public RedisRepository(IConnectionMultiplexer redisMultiplexer)
        {
            _db = redisMultiplexer.GetDatabase();
        }

        public async Task AddAsync(string key, string body)
        {
            await _db.StringSetAsync(key, body);
        }

        public async Task<string> GetAsync(string key)
        {
            return await _db.StringGetAsync(key);
        }

        public async Task DeleteAsync(string key)
        {
            await _db.KeyDeleteAsync(key);
        }

        public async Task<bool> AnyAsync(string key)
        {
            return await _db.KeyExistsAsync(key);
        }
    }
}
