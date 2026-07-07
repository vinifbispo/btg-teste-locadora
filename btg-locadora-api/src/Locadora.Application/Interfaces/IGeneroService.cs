using Locadora.Application.Dtos;

namespace Locadora.Application.Interfaces;

public interface IGeneroService
{
    Task<IEnumerable<GeneroDto>> BuscarPorNomeAsync(string termo);
}
