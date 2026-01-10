using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WishList.API.Data.Models.DTO;
using WishList.API.Services.Interfaces;

namespace WishList.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class WishesController : ControllerBase
{
    private readonly IWishService _wishService;
    private readonly ILogger<WishesController> _logger;

    public WishesController(IWishService wishService, ILogger<WishesController> logger)
    {
        _wishService = wishService;
        _logger = logger;
    }

    /// <summary>
    /// Get paginated list of wishes with optional search
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponseDto<WishResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PagedResponseDto<WishResponseDto>>> GetWishes(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null)
    {
        var result = await _wishService.GetPagedAsync(page, pageSize, search, User);
        return Ok(result);
    }

    /// <summary>
    /// Get wish by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(WishResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<WishResponseDto>> GetWish(Guid id)
    {
        var result = await _wishService.GetByIdAsync(id, User);
        return Ok(result);
    }

    /// <summary>
    /// Create a new wish
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(WishResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<WishResponseDto>> CreateWish([FromBody] CreateWishDto dto)
    {
        var result = await _wishService.CreateAsync(dto, User);
        return CreatedAtAction(nameof(GetWish), new { id = result.Id }, result);
    }

    /// <summary>
    /// Update an existing wish
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(WishResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<WishResponseDto>> UpdateWish(Guid id, [FromBody] UpdateWishDto dto)
    {
        var result = await _wishService.UpdateAsync(id, dto, User);
        return Ok(result);
    }

    /// <summary>
    /// Delete a wish
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteWish(Guid id)
    {
        await _wishService.DeleteAsync(id, User);
        return NoContent();
    }

    /// <summary>
    /// Get recommended wishes for the current user
    /// </summary>
    [HttpGet("recommendations")]
    [ProducesResponseType(typeof(List<WishResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<WishResponseDto>>> GetRecommendations()
    {
        var result = await _wishService.GetRecommendationsAsync(User);
        return Ok(result);
    }
}

