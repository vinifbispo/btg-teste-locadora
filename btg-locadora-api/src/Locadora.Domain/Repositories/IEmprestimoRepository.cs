using Locadora.Domain.Entities;

namespace Locadora.Domain.Repositories;

public interface IEmprestimoRepository
{
    Task<IQueryable<Emprestimo>> ListarAsync();

    Task<Emprestimo?> ObterPorIdAsync(int id);

    Task<Emprestimo?> ObterAtivoPorJogoAsync(int jogoId);

    Task AdicionarAsync(Emprestimo emprestimo);

    Task AtualizarAsync(Emprestimo emprestimo);
}
