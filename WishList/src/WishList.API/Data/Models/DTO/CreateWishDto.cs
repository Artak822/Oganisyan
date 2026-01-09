namespace WishList.API.Data.Models.DTO;

public class CreateWishDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Link { get; set; }
    public decimal? Price { get; set; }
    public bool IsPublic { get; set; } = true;
    public string? IdempotencyKey { get; set; }
}

