using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WishList.API.Data.Models.Entities;

public class Wish
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;
    
    [MaxLength(1000)]
    public string? Description { get; set; }
    
    [MaxLength(500)]
    public string? Link { get; set; }
    
    [Column(TypeName = "decimal(10,2)")]
    public decimal? Price { get; set; }
    
    [MaxLength(100)]
    public string? IdempotencyKey { get; set; }
    
    [Required]
    public Guid CreatedByUserId { get; set; }
    
    [Required]
    public bool IsPublic { get; set; } = true; // If true, appears in recommendations
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    [ForeignKey("CreatedByUserId")]
    public User CreatedByUser { get; set; } = null!;
    
    public ICollection<WishRecommendation> RecommendedToUsers { get; set; } = new List<WishRecommendation>();
}

