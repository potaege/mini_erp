using MiniErp.Api.Models;

namespace MiniErp.Api.Repositories;

public interface IUserRepository
{
    Task<User?> GetByNameAsync(string name, CancellationToken cancellationToken);
}
