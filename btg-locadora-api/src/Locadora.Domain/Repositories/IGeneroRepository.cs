using Locadora.Domain.Entities;

namespace Locadora.Domain.Repositories;

public interface IGeneroRepository
{
    Task<List<Genero>> ListarPorIdsAsync(IEnumerable<int> ids);

    Task<List<Genero>> ObterOuCriarPorNomesAsync(IEnumerable<string> nomes);

    Task<List<Genero>> BuscarPorNomeAsync(string termo);
}
