using Locadora.Domain.Entities;

namespace Locadora.Domain.Caching;

public interface IEmprestimoCache
{
    Task<Emprestimo?> ObterAsync(int id);

    Task DefinirAsync(Emprestimo emprestimo);

    Task RemoverAsync(int id);
}
