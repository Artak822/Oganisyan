using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WishList.API.Data;
using WishList.API.Data.Models.Entities;
using WishList.API.Repositories.Interfaces;

namespace WishList.API.Repositories;

public class ApiKeyRepository : IApiKeyRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ApiKeyRepository> _logger;

    public ApiKeyRepository(
        ApplicationDbContext context,
        ILogger<ApiKeyRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ApiKey?> GetByKeyAsync(string key)
    {
        return await _context.ApiKeys
            .FirstOrDefaultAsync(ak => ak.Key == key && ak.IsActive);
    }

    public async Task<ApiKey> CreateAsync(ApiKey apiKey)
    {
        _context.ApiKeys.Add(apiKey);
        await _context.SaveChangesAsync();
        return apiKey;
    }

    public async Task<List<ApiKey>> GetAllAsync()
    {
        return await _context.ApiKeys.ToListAsync();
    }

    public async Task<bool> ExistsAsync(string key)
    {
        return await _context.ApiKeys.AnyAsync(ak => ak.Key == key);
    }
}

