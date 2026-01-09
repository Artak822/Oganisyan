using System.Data;

namespace WishList.API.Repositories.Interfaces;

public interface IDapperContext
{
    IDbConnection CreateConnection();
}

