using Locadora.Domain.Entities;

namespace Locadora.Domain.Repositories;

public interface IEmprestimoRepository
{
    Task<IQueryable<Emprestimo>> ListarAsync();

    Task<Emprestimo?> ObterPorIdAsync(int id);

    Task<Emprestimo?> ObterAtivoPorJogoAsync(int jogoId);

    Task<bool> ExisteParaAmigoAsync(int amigoId);

    Task<bool> ExisteParaJogoAsync(int jogoId);

    Task AdicionarAsync(Emprestimo emprestimo);

    Task AtualizarAsync(Emprestimo emprestimo);
}
