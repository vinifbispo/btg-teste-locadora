using Locadora.Domain.Entities;

namespace Locadora.Domain.Caching;

public interface IAmigoCache
{
    Task<Amigo?> ObterAsync(int id);

    Task DefinirAsync(Amigo amigo);

    Task RemoverAsync(int id);
}
