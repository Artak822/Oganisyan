using WishList.API.Data.Models.Entities;

namespace WishList.API.Services.Interfaces;

public interface IApiKeyService
{
    Task<ApiKey?> ValidateApiKeyAsync(string key);
    Task<ApiKey> CreateApiKeyAsync(string name, DateTime expiresAt);
    Task<List<ApiKey>> GetAllApiKeysAsync();
}

