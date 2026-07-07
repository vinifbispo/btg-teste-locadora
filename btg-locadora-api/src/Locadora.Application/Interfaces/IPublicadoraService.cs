using Locadora.Application.Dtos;

namespace Locadora.Application.Interfaces;

public interface IPublicadoraService
{
    Task<IEnumerable<PublicadoraDto>> BuscarPorNomeAsync(string termo);
}
