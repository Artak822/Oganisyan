using WishList.API.Data.Models.Entities;

namespace WishList.API.Repositories.Interfaces;

public interface IApiKeyRepository
{
    Task<ApiKey?> GetByKeyAsync(string key);
    Task<ApiKey> CreateAsync(ApiKey apiKey);
    Task<List<ApiKey>> GetAllAsync();
    Task<bool> ExistsAsync(string key);
}

