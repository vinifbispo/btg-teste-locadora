using Locadora.Application.Dtos;
using Locadora.Application.Mappings;
using Locadora.Domain.Repositories;
using MediatR;

namespace Locadora.Application.Queries.Publicadoras.BuscarPublicadorasPorNome;

public class BuscarPublicadorasPorNomeQueryHandler : IRequestHandler<BuscarPublicadorasPorNomeQuery, IEnumerable<PublicadoraDto>>
{
    private readonly IPublicadoraQueryRepository _publicadoras;

    public BuscarPublicadorasPorNomeQueryHandler(IPublicadoraQueryRepository publicadoras)
    {
        _publicadoras = publicadoras;
    }

    public async Task<IEnumerable<PublicadoraDto>> Handle(BuscarPublicadorasPorNomeQuery request, CancellationToken cancellationToken)
    {
        var publicadoras = await _publicadoras.BuscarPorNomeAsync(request.Termo);
        return publicadoras.Select(p => p.ToDto()).ToList();
    }
}
