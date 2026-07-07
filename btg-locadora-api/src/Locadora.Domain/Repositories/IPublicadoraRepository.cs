using Locadora.Domain.Entities;

namespace Locadora.Domain.Repositories;

public interface IPublicadoraRepository
{
    Task<List<Publicadora>> ListarPorIdsAsync(IEnumerable<int> ids);

    Task<List<Publicadora>> ObterOuCriarPorNomesAsync(IEnumerable<string> nomes);

    Task<List<Publicadora>> BuscarPorNomeAsync(string termo);
}
