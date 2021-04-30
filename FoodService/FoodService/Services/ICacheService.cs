using System.Threading.Tasks;
using StackExchange.Redis;

namespace FoodService.Services
{
    public interface ICacheService
    {
        Task<HashEntry[]> GetHashAsync(string key);
    }
}