using Locadora.Domain.Entities;

namespace Locadora.Domain.Repositories;

public interface IAmigoRepository
{
    Task<IQueryable<Amigo>> ListarAsync();

    Task<Amigo?> ObterPorIdAsync(int id);

    Task<bool> ExisteAsync(int id);

    Task AdicionarAsync(Amigo amigo);

    Task AtualizarAsync(Amigo amigo);

    Task RemoverAsync(Amigo amigo);
}
