using Locadora.Domain.Entities;

namespace Locadora.Domain.Repositories;

public interface IEmprestimoQueryRepository
{
    Task<IQueryable<Emprestimo>> ListarAsync();

    Task<Emprestimo?> ObterPorIdAsync(int id);
}
