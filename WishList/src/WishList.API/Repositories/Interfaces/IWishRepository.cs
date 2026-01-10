using WishList.API.Data.Models.Entities;
using WishList.API.Data.Models.DTO;

namespace WishList.API.Repositories.Interfaces;

public interface IWishRepository
{
    Task<Wish?> GetByIdAsync(Guid id);
    Task<PagedResponseDto<Wish>> GetPagedAsync(int page, int pageSize, string? search);
    Task<Wish> CreateAsync(Wish wish);
    Task<Wish> UpdateAsync(Wish wish);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<List<Wish>> GetPublicWishesAsync();
    Task<Wish?> GetByIdempotencyKeyAsync(string idempotencyKey, Guid userId);
}

