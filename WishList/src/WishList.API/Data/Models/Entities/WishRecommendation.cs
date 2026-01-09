using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WishList.API.Data.Models.Entities;

// Many-to-many relationship between User and Wish
public class WishRecommendation
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid UserId { get; set; }
    
    [Required]
    public Guid WishId { get; set; }
    
    public bool IsViewed { get; set; } = false;
    
    public DateTime RecommendedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    [ForeignKey("UserId")]
    public User User { get; set; } = null!;
    
    [ForeignKey("WishId")]
    public Wish Wish { get; set; } = null!;
}

