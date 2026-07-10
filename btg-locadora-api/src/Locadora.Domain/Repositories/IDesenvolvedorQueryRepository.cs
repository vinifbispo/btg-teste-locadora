using Locadora.Domain.Entities;

namespace Locadora.Domain.Repositories;

public interface IDesenvolvedorQueryRepository
{
    Task<List<Desenvolvedor>> BuscarPorNomeAsync(string termo);
}
