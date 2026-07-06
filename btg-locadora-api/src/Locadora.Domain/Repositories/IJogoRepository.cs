using Locadora.Domain.Entities;

namespace Locadora.Domain.Repositories;

public interface IJogoRepository
{
    Task<IQueryable<Jogo>> ListarAsync();

    Task<Jogo?> ObterPorIdAsync(int id);

    Task AdicionarAsync(Jogo jogo);

    Task AtualizarAsync(Jogo jogo);

    Task RemoverAsync(Jogo jogo);
}
