using System.Data;
using Npgsql;
using WishList.API.Repositories.Interfaces;

namespace WishList.API.Data;

public class DapperContext : IDapperContext
{
    private readonly string _connectionString;

    public DapperContext(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string not found");
    }

    public IDbConnection CreateConnection() => new NpgsqlConnection(_connectionString);
}

