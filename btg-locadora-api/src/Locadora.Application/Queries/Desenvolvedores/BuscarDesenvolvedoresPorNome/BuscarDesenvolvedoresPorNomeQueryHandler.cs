using Locadora.Application.Dtos;
using Locadora.Application.Mappings;
using Locadora.Domain.Repositories;
using MediatR;

namespace Locadora.Application.Queries.Desenvolvedores.BuscarDesenvolvedoresPorNome;

public class BuscarDesenvolvedoresPorNomeQueryHandler : IRequestHandler<BuscarDesenvolvedoresPorNomeQuery, IEnumerable<DesenvolvedorDto>>
{
    private readonly IDesenvolvedorRepository _desenvolvedores;

    public BuscarDesenvolvedoresPorNomeQueryHandler(IDesenvolvedorRepository desenvolvedores)
    {
        _desenvolvedores = desenvolvedores;
    }

    public async Task<IEnumerable<DesenvolvedorDto>> Handle(BuscarDesenvolvedoresPorNomeQuery request, CancellationToken cancellationToken)
    {
        var desenvolvedores = await _desenvolvedores.BuscarPorNomeAsync(request.Termo);
        return desenvolvedores.Select(d => d.ToDto()).ToList();
    }
}
