using Locadora.Domain.Entities;

namespace Locadora.Domain.Repositories;

public interface IPublicadoraQueryRepository
{
    Task<List<Publicadora>> BuscarPorNomeAsync(string termo);
}
