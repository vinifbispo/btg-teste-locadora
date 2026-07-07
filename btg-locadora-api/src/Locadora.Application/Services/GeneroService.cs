using Locadora.Application.Dtos;
using Locadora.Application.Interfaces;
using Locadora.Application.Mappings;
using Locadora.Domain.Repositories;

namespace Locadora.Application.Services;

public class GeneroService : IGeneroService
{
    private readonly IGeneroRepository _generos;

    public GeneroService(IGeneroRepository generos)
    {
        _generos = generos;
    }

    public async Task<IEnumerable<GeneroDto>> BuscarPorNomeAsync(string termo)
    {
        var generos = await _generos.BuscarPorNomeAsync(termo);
        return generos.Select(g => g.ToDto()).ToList();
    }
}
