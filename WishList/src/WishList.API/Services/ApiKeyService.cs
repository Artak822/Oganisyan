using Microsoft.Extensions.Logging;
using WishList.API.Data.Models.Entities;
using WishList.API.Repositories.Interfaces;
using WishList.API.Services.Interfaces;

namespace WishList.API.Services;

public class ApiKeyService : IApiKeyService
{
    private readonly IApiKeyRepository _apiKeyRepository;
    private readonly ILogger<ApiKeyService> _logger;

    public ApiKeyService(
        IApiKeyRepository apiKeyRepository,
        ILogger<ApiKeyService> logger)
    {
        _apiKeyRepository = apiKeyRepository;
        _logger = logger;
    }

    public async Task<ApiKey?> ValidateApiKeyAsync(string key)
    {
        var apiKey = await _apiKeyRepository.GetByKeyAsync(key);
        
        if (apiKey == null)
        {
            return null;
        }

        if (!apiKey.IsActive)
        {
            _logger.LogWarning("Inactive API key attempted: {Key}", key);
            return null;
        }

        if (apiKey.ExpiresAt < DateTime.UtcNow)
        {
            _logger.LogWarning("Expired API key attempted: {Key}", key);
            return null;
        }

        return apiKey;
    }

    public async Task<ApiKey> CreateApiKeyAsync(string name, DateTime expiresAt)
    {
        var key = GenerateApiKey();
        
        // Ensure uniqueness
        while (await _apiKeyRepository.ExistsAsync(key))
        {
            key = GenerateApiKey();
        }

        var apiKey = new ApiKey
        {
            Key = key,
            Name = name,
            ExpiresAt = expiresAt,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        return await _apiKeyRepository.CreateAsync(apiKey);
    }

    public async Task<List<ApiKey>> GetAllApiKeysAsync()
    {
        return await _apiKeyRepository.GetAllAsync();
    }

    private string GenerateApiKey()
    {
        return Convert.ToBase64String(Guid.NewGuid().ToByteArray())
            .Replace("/", "_")
            .Replace("+", "-")
            .TrimEnd('=');
    }
}

