using Locadora.Domain.Entities;

namespace Locadora.Domain.Repositories;

public interface IDesenvolvedorRepository
{
    Task<List<Desenvolvedor>> ListarPorIdsAsync(IEnumerable<int> ids);

    Task<List<Desenvolvedor>> ObterOuCriarPorNomesAsync(IEnumerable<string> nomes);
}
