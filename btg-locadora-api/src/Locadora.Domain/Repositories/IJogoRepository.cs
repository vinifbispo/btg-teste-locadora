using Locadora.Domain.Entities;

namespace Locadora.Domain.Repositories;

public interface IJogoRepository
{
    Task<IQueryable<Jogo>> ListarAsync();

    Task<Jogo?> ObterPorIdAsync(int id);

    Task AdicionarAsync(Jogo jogo);

    Task AdicionarVariosAsync(IEnumerable<Jogo> jogos);

    Task AtualizarAsync(Jogo jogo);

    Task RemoverAsync(Jogo jogo);
}
