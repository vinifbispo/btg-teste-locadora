using Locadora.Domain.Entities;

namespace Locadora.Domain.Repositories;

public interface IGeneroQueryRepository
{
    Task<List<Genero>> BuscarPorNomeAsync(string termo);
}
