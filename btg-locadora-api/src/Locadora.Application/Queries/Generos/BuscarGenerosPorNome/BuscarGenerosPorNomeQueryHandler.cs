using Locadora.Application.Dtos;
using Locadora.Application.Mappings;
using Locadora.Domain.Repositories;
using MediatR;

namespace Locadora.Application.Queries.Generos.BuscarGenerosPorNome;

public class BuscarGenerosPorNomeQueryHandler : IRequestHandler<BuscarGenerosPorNomeQuery, IEnumerable<GeneroDto>>
{
    private readonly IGeneroRepository _generos;

    public BuscarGenerosPorNomeQueryHandler(IGeneroRepository generos)
    {
        _generos = generos;
    }

    public async Task<IEnumerable<GeneroDto>> Handle(BuscarGenerosPorNomeQuery request, CancellationToken cancellationToken)
    {
        var generos = await _generos.BuscarPorNomeAsync(request.Termo);
        return generos.Select(g => g.ToDto()).ToList();
    }
}
