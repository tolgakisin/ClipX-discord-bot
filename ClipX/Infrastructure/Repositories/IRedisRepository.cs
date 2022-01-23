using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClipX.Infrastructure.Repositories
{
    public interface IRedisRepository
    {
        Task AddAsync(string key, string body);
        Task<string> GetAsync(string key);
        Task DeleteAsync(string key);
        Task<bool> AnyAsync(string key);
    }
}
