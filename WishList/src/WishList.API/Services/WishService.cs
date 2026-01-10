using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using WishList.API.Data;
using WishList.API.Data.Models.DTO;
using WishList.API.Data.Models.Entities;
using WishList.API.Repositories.Interfaces;
using WishList.API.Services.Interfaces;

namespace WishList.API.Services;

public class WishService : IWishService
{
    private readonly IWishRepository _wishRepository;
    private readonly IUserRepository _userRepository;
    private readonly ApplicationDbContext _context;
    private readonly ICacheService _cacheService;
    private readonly ILogger<WishService> _logger;

    public WishService(
        IWishRepository wishRepository,
        IUserRepository userRepository,
        ApplicationDbContext context,
        ICacheService cacheService,
        ILogger<WishService> logger)
    {
        _wishRepository = wishRepository;
        _userRepository = userRepository;
        _context = context;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<WishResponseDto> GetByIdAsync(Guid id, ClaimsPrincipal? user)
    {
        if (!await CanReadAsync(user))
        {
            throw new UnauthorizedAccessException("You don't have permission to read wishes");
        }

        var cacheKey = $"wish:{id}";
        var cached = await _cacheService.GetAsync<WishResponseDto>(cacheKey);
        if (cached != null)
        {
            return cached;
        }

        var wish = await _wishRepository.GetByIdAsync(id);
        if (wish == null)
        {
            throw new KeyNotFoundException($"Wish with id {id} not found");
        }

        var dto = MapToDto(wish);
        await _cacheService.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(5));
        
        return dto;
    }

    public async Task<PagedResponseDto<WishResponseDto>> GetPagedAsync(int page, int pageSize, string? search, ClaimsPrincipal? user)
    {
        if (!await CanReadAsync(user))
        {
            throw new UnauthorizedAccessException("You don't have permission to read wishes");
        }

        var normalizedSearch = search?.Trim().ToLowerInvariant() ?? string.Empty;
        var cacheKey = $"wishes:{page}:{pageSize}:{normalizedSearch}";

        var cached = await _cacheService.GetAsync<PagedResponseDto<WishResponseDto>>(cacheKey);
        if (cached != null)
        {
            return cached;
        }

        var pagedResult = await _wishRepository.GetPagedAsync(page, pageSize, search);

        var response = new PagedResponseDto<WishResponseDto>
        {
            Items = pagedResult.Items.Select(MapToDto).ToList(),
            Total = pagedResult.Total,
            Page = pagedResult.Page,
            PageSize = pagedResult.PageSize
        };

        await _cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(1));
        return response;
    }

    public async Task<WishResponseDto> CreateAsync(CreateWishDto dto, ClaimsPrincipal user)
    {
        if (!await CanCreateAsync(user))
        {
            throw new UnauthorizedAccessException("You don't have permission to create wishes");
        }

        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("Invalid user token");
        }

        // Check idempotency
        if (!string.IsNullOrEmpty(dto.IdempotencyKey))
        {
            var existing = await _wishRepository.GetByIdempotencyKeyAsync(dto.IdempotencyKey, userId);
            if (existing != null)
            {
                _logger.LogInformation("Idempotent request detected for key: {Key}", dto.IdempotencyKey);
                return MapToDto(existing);
            }
        }

        var wish = new Wish
        {
            Title = dto.Title,
            Description = dto.Description,
            Link = dto.Link,
            Price = dto.Price,
            IsPublic = dto.IsPublic,
            CreatedByUserId = userId,
            IdempotencyKey = dto.IdempotencyKey,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _wishRepository.CreateAsync(wish);
        
        // If public, add to recommendations for all users
        if (wish.IsPublic)
        {
            await AddToRecommendationsAsync(created.Id);
        }

        // Invalidate cache
        await _cacheService.RemoveByPatternAsync("wish:*");
        await _cacheService.RemoveByPatternAsync("wishes:*");

        _logger.LogInformation("Wish created: {WishId} by user {UserId}", created.Id, userId);
        
        return MapToDto(created);
    }

    public async Task<WishResponseDto> UpdateAsync(Guid id, UpdateWishDto dto, ClaimsPrincipal user)
    {
        if (!await CanUpdateAsync(user, id))
        {
            throw new UnauthorizedAccessException("You don't have permission to update this wish");
        }

        var wish = await _wishRepository.GetByIdAsync(id);
        if (wish == null)
        {
            throw new KeyNotFoundException($"Wish with id {id} not found");
        }

        if (!string.IsNullOrEmpty(dto.Title))
            wish.Title = dto.Title;
        if (dto.Description != null)
            wish.Description = dto.Description;
        if (dto.Link != null)
            wish.Link = dto.Link;
        if (dto.Price.HasValue)
            wish.Price = dto.Price;
        if (dto.IsPublic.HasValue)
        {
            var wasPublic = wish.IsPublic;
            wish.IsPublic = dto.IsPublic.Value;
            
            // If became public, add to recommendations
            if (!wasPublic && wish.IsPublic)
            {
                await AddToRecommendationsAsync(wish.Id);
            }
        }

        wish.UpdatedAt = DateTime.UtcNow;
        var updated = await _wishRepository.UpdateAsync(wish);

        // Invalidate cache
        await _cacheService.RemoveAsync($"wish:{id}");
        await _cacheService.RemoveByPatternAsync("wishes:*");

        _logger.LogInformation("Wish updated: {WishId}", id);
        
        return MapToDto(updated);
    }

    public async Task DeleteAsync(Guid id, ClaimsPrincipal user)
    {
        if (!await CanDeleteAsync(user, id))
        {
            throw new UnauthorizedAccessException("You don't have permission to delete this wish");
        }

        var wish = await _wishRepository.GetByIdAsync(id);
        if (wish == null)
        {
            throw new KeyNotFoundException($"Wish with id {id} not found");
        }

        await _wishRepository.DeleteAsync(id);

        // Invalidate cache
        await _cacheService.RemoveAsync($"wish:{id}");
        await _cacheService.RemoveByPatternAsync("wishes:*");
        await _cacheService.RemoveByPatternAsync("recommendations:*");

        _logger.LogInformation("Wish deleted: {WishId}", id);
    }

    public async Task<List<WishResponseDto>> GetRecommendationsAsync(ClaimsPrincipal user)
    {
        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("Invalid user token");
        }

        var cacheKey = $"recommendations:{userId}";
        var cached = await _cacheService.GetAsync<List<WishResponseDto>>(cacheKey);
        if (cached != null)
        {
            return cached;
        }

        var recommendations = await _context.WishRecommendations
            .Include(wr => wr.Wish)
            .ThenInclude(w => w.CreatedByUser)
            .Where(wr => wr.UserId == userId && !wr.IsViewed)
            .Select(wr => wr.Wish)
            .ToListAsync();

        var dtos = recommendations.Select(MapToDto).ToList();
        await _cacheService.SetAsync(cacheKey, dtos, TimeSpan.FromMinutes(10));
        
        return dtos;
    }

    private async Task AddToRecommendationsAsync(Guid wishId)
    {
        var allUsers = await _context.Users.Select(u => u.Id).ToListAsync();
        var existingRecommendations = await _context.WishRecommendations
            .Where(wr => wr.WishId == wishId)
            .Select(wr => wr.UserId)
            .ToListAsync();

        var newRecommendations = allUsers
            .Where(uid => !existingRecommendations.Contains(uid))
            .Select(uid => new WishRecommendation
            {
                UserId = uid,
                WishId = wishId,
                RecommendedAt = DateTime.UtcNow
            })
            .ToList();

        if (newRecommendations.Any())
        {
            _context.WishRecommendations.AddRange(newRecommendations);
            await _context.SaveChangesAsync();
            await _cacheService.RemoveByPatternAsync("recommendations:*");
        }
    }

    public Task<bool> CanReadAsync(ClaimsPrincipal? user)
    {
        if (user == null) return Task.FromResult(false);
        
        // Check if this is an API Key authentication (system access)
        var isApiKey = user.HasClaim("ApiKey", "true");
        if (isApiKey)
        {
            return Task.FromResult(true);
        }
        
        var role = user.FindFirst(ClaimTypes.Role)?.Value;
        var hasAccess = role == "Admin" || role == "Manager" || role == "User";
        return Task.FromResult(hasAccess);
    }

    public Task<bool> CanCreateAsync(ClaimsPrincipal? user)
    {
        if (user == null)
        {
            _logger.LogWarning("CanCreateAsync: user is null");
            return Task.FromResult(false);
        }
        
        // Check if this is an API Key authentication (system access)
        var isApiKey = user.HasClaim("ApiKey", "true");
        if (isApiKey)
        {
            _logger.LogInformation("CanCreateAsync: API Key authentication detected, allowing access");
            return Task.FromResult(true);
        }
        
        var role = user.FindFirst(ClaimTypes.Role)?.Value;
        var allClaims = user.Claims.Select(c => $"{c.Type}={c.Value}").ToList();
        _logger.LogInformation("CanCreateAsync: Role={Role}, AllClaims=[{Claims}]", 
            role ?? "null", string.Join(", ", allClaims));
        
        var hasAccess = role == "Admin" || role == "Manager" || role == "User";
        return Task.FromResult(hasAccess);
    }

    public async Task<bool> CanUpdateAsync(ClaimsPrincipal? user, Guid wishId)
    {
        if (user == null) return false;
        var role = user.FindFirst(ClaimTypes.Role)?.Value;
        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (role == "Admin") return true;
        if (role == "Manager") return true;

        if (role == "User" && Guid.TryParse(userIdClaim, out var userId))
        {
            var wish = await _wishRepository.GetByIdAsync(wishId);
            return wish?.CreatedByUserId == userId;
        }

        return false;
    }

    public async Task<bool> CanDeleteAsync(ClaimsPrincipal? user, Guid wishId)
    {
        if (user == null) return false;
        var role = user.FindFirst(ClaimTypes.Role)?.Value;
        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (role == "Admin") return true;

        if (role == "User" && Guid.TryParse(userIdClaim, out var userId))
        {
            var wish = await _wishRepository.GetByIdAsync(wishId);
            return wish?.CreatedByUserId == userId;
        }

        return false;
    }

    private WishResponseDto MapToDto(Wish wish)
    {
        return new WishResponseDto
        {
            Id = wish.Id,
            Title = wish.Title,
            Description = wish.Description,
            Link = wish.Link,
            Price = wish.Price,
            CreatedByUserId = wish.CreatedByUserId,
            CreatedByUsername = wish.CreatedByUser?.Username ?? string.Empty,
            IsPublic = wish.IsPublic,
            CreatedAt = wish.CreatedAt,
            UpdatedAt = wish.UpdatedAt
        };
    }
}

