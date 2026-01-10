using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WishList.API.Data;
using WishList.API.Data.Models.Entities;
using WishList.API.Data.Models.DTO;
using WishList.API.Repositories.Interfaces;
using System.Data;
using System.Data.Common;
using Dapper;

namespace WishList.API.Repositories;

public class WishRepository : IWishRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IDapperContext _dapperContext;
    private readonly ILogger<WishRepository> _logger;

    public WishRepository(
        ApplicationDbContext context,
        IDapperContext dapperContext,
        ILogger<WishRepository> logger)
    {
        _context = context;
        _dapperContext = dapperContext;
        _logger = logger;
    }

    public async Task<Wish?> GetByIdAsync(Guid id)
    {
        return await _context.Wishes
            .Include(w => w.CreatedByUser)
            .FirstOrDefaultAsync(w => w.Id == id);
    }

    public async Task<PagedResponseDto<Wish>> GetPagedAsync(int page, int pageSize, string? search)
    {
        var query = _context.Wishes
            .Include(w => w.CreatedByUser)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.ToLower();
            query = query.Where(w =>
                w.Title.ToLower().Contains(search) ||
                (w.Description != null && w.Description.ToLower().Contains(search)));
        }

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(w => w.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResponseDto<Wish>
        {
            Items = items,
            Total = total,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<Wish> CreateAsync(Wish wish)
    {
        _context.Wishes.Add(wish);
        await _context.SaveChangesAsync();
        return wish;
    }

    public async Task<Wish> UpdateAsync(Wish wish)
    {
        wish.UpdatedAt = DateTime.UtcNow;
        _context.Wishes.Update(wish);
        await _context.SaveChangesAsync();
        return wish;
    }

    public async Task DeleteAsync(Guid id)
    {
        var wish = await _context.Wishes.FindAsync(id);
        if (wish != null)
        {
            _context.Wishes.Remove(wish);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Wishes.AnyAsync(w => w.Id == id);
    }

    public async Task<List<Wish>> GetPublicWishesAsync()
    {
        return await _context.Wishes
            .Include(w => w.CreatedByUser)
            .Where(w => w.IsPublic)
            .OrderByDescending(w => w.CreatedAt)
            .ToListAsync();
    }

   public async Task<Wish?> GetByIdempotencyKeyAsync(string idempotencyKey, Guid userId)
{
        await using var dbConnection = _dapperContext.CreateConnection() as DbConnection
            ?? throw new InvalidOperationException("Unable to create a database connection for idempotency lookup.");

        await dbConnection.OpenAsync();

        await using var transaction = await dbConnection.BeginTransactionAsync();
        try
        {
            var sql = @"
                SELECT w.""Id"", w.""Title"", w.""Description"", w.""Link"", w.""Price"", 
                       w.""IdempotencyKey"", w.""CreatedByUserId"", w.""IsPublic"", 
                       w.""CreatedAt"", w.""UpdatedAt""
                FROM ""Wishes"" w
                WHERE w.""IdempotencyKey"" = @IdempotencyKey 
                  AND w.""CreatedByUserId"" = @UserId
                LIMIT 1";

            var result = await dbConnection.QueryFirstOrDefaultAsync<Wish>(sql, new
            {
                IdempotencyKey = idempotencyKey,
                UserId = userId
            }, transaction);

            await transaction.CommitAsync();
            return result;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
}  
}

