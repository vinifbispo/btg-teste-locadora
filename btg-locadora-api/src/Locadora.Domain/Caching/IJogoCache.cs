using Locadora.Domain.Entities;

namespace Locadora.Domain.Caching;

public interface IJogoCache
{
    Task<Jogo?> ObterAsync(int id);

    Task DefinirAsync(Jogo jogo);

    Task RemoverAsync(int id);
}
