using Locadora.Domain.Entities;

namespace Locadora.Domain.Repositories;

public interface IAmigoQueryRepository
{
    Task<IQueryable<Amigo>> ListarAsync();

    Task<Amigo?> ObterPorIdAsync(int id);
}
