using WishList.API.Data.Models.DTO;
using System.Security.Claims;

namespace WishList.API.Services.Interfaces;

public interface IAuthService
{
    Task<TokenResponseDto> RegisterAsync(RegisterDto dto);
    Task<TokenResponseDto> LoginAsync(LoginDto dto);
    ClaimsPrincipal? ValidateToken(string token);
}

