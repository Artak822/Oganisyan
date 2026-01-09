using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using WishList.API.Data;
using WishList.API.Data.Models.Entities;
using WishList.API.Repositories;
using WishList.API.Repositories.Interfaces;
using Xunit;

namespace WishList.Tests.Repositories;

public class WishRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<IDapperContext> _dapperContextMock;
    private readonly Mock<ILogger<WishRepository>> _loggerMock;
    private readonly WishRepository _repository;

    public WishRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _dapperContextMock = new Mock<IDapperContext>();
        _loggerMock = new Mock<ILogger<WishRepository>>();
        _repository = new WishRepository(_context, _dapperContextMock.Object, _loggerMock.Object);

        // Seed test data
        SeedTestData();
    }

    private void SeedTestData()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = "hash",
            Role = "User",
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);

        var wish = new Wish
        {
            Id = Guid.NewGuid(),
            Title = "Test Wish",
            Description = "Test Description",
            CreatedByUserId = user.Id,
            IsPublic = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Wishes.Add(wish);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsWish()
    {
        // Arrange
        var wish = _context.Wishes.First();

        // Act
        var result = await _repository.GetByIdAsync(wish.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(wish.Id);
        result.Title.Should().Be(wish.Title);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingId_ReturnsNull()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();

        // Act
        var result = await _repository.GetByIdAsync(nonExistingId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_ValidWish_CreatesAndReturnsWish()
    {
        // Arrange
        var user = _context.Users.First();
        var newWish = new Wish
        {
            Title = "New Wish",
            Description = "New Description",
            CreatedByUserId = user.Id,
            IsPublic = true,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var result = await _repository.CreateAsync(newWish);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.Title.Should().Be("New Wish");
        
        var savedWish = await _context.Wishes.FindAsync(result.Id);
        savedWish.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateAsync_ExistingWish_UpdatesAndReturnsWish()
    {
        // Arrange
        var wish = _context.Wishes.First();
        wish.Title = "Updated Title";

        // Act
        var result = await _repository.UpdateAsync(wish);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be("Updated Title");
        result.UpdatedAt.Should().NotBeNull();
        
        var updatedWish = await _context.Wishes.FindAsync(wish.Id);
        updatedWish!.Title.Should().Be("Updated Title");
    }

    [Fact]
    public async Task DeleteAsync_ExistingId_DeletesWish()
    {
        // Arrange
        var wish = _context.Wishes.First();
        var wishId = wish.Id;

        // Act
        await _repository.DeleteAsync(wishId);

        // Assert
        var deletedWish = await _context.Wishes.FindAsync(wishId);
        deletedWish.Should().BeNull();
    }

    [Fact]
    public async Task ExistsAsync_ExistingId_ReturnsTrue()
    {
        // Arrange
        var wish = _context.Wishes.First();

        // Act
        var result = await _repository.ExistsAsync(wish.Id);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_NonExistingId_ReturnsFalse()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();

        // Act
        var result = await _repository.ExistsAsync(nonExistingId);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetPagedAsync_WithSearch_ReturnsFilteredResults()
    {
        // Arrange
        var user = _context.Users.First();
        _context.Wishes.AddRange(
            new Wish { Title = "Apple iPhone", CreatedByUserId = user.Id, IsPublic = true, CreatedAt = DateTime.UtcNow },
            new Wish { Title = "Samsung Galaxy", CreatedByUserId = user.Id, IsPublic = true, CreatedAt = DateTime.UtcNow },
            new Wish { Title = "Laptop", CreatedByUserId = user.Id, IsPublic = true, CreatedAt = DateTime.UtcNow }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetPagedAsync(1, 10, "Apple");

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().Contain(w => w.Title.Contains("Apple"));
        result.Items.Should().NotContain(w => w.Title.Contains("Samsung"));
    }

    [Fact]
    public async Task GetPagedAsync_WithoutSearch_ReturnsAllResults()
    {
        // Act
        var result = await _repository.GetPagedAsync(1, 10, null);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetPublicWishesAsync_ReturnsOnlyPublicWishes()
    {
        // Arrange
        var user = _context.Users.First();
        _context.Wishes.AddRange(
            new Wish { Title = "Public Wish", CreatedByUserId = user.Id, IsPublic = true, CreatedAt = DateTime.UtcNow },
            new Wish { Title = "Private Wish", CreatedByUserId = user.Id, IsPublic = false, CreatedAt = DateTime.UtcNow }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetPublicWishesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().OnlyContain(w => w.IsPublic);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

