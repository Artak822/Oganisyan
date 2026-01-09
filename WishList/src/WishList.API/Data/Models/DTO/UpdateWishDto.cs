namespace WishList.API.Data.Models.DTO;

public class UpdateWishDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Link { get; set; }
    public decimal? Price { get; set; }
    public bool? IsPublic { get; set; }
}

