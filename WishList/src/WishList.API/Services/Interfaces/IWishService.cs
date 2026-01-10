using WishList.API.Data.Models.DTO;
using System.Security.Claims;

namespace WishList.API.Services.Interfaces;

public interface IWishService
{
    Task<WishResponseDto> GetByIdAsync(Guid id, ClaimsPrincipal? user);
    Task<PagedResponseDto<WishResponseDto>> GetPagedAsync(int page, int pageSize, string? search, ClaimsPrincipal? user);
    Task<WishResponseDto> CreateAsync(CreateWishDto dto, ClaimsPrincipal user);
    Task<WishResponseDto> UpdateAsync(Guid id, UpdateWishDto dto, ClaimsPrincipal user);
    Task DeleteAsync(Guid id, ClaimsPrincipal user);
    Task<List<WishResponseDto>> GetRecommendationsAsync(ClaimsPrincipal user);
    Task<bool> CanReadAsync(ClaimsPrincipal? user);
    Task<bool> CanCreateAsync(ClaimsPrincipal? user);
    Task<bool> CanUpdateAsync(ClaimsPrincipal? user, Guid wishId);
    Task<bool> CanDeleteAsync(ClaimsPrincipal? user, Guid wishId);
}

