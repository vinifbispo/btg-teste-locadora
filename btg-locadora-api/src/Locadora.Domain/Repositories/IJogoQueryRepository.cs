using Locadora.Domain.Entities;

namespace Locadora.Domain.Repositories;

public interface IJogoQueryRepository
{
    Task<IQueryable<Jogo>> ListarAsync();

    Task<Jogo?> ObterPorIdAsync(int id);
}
