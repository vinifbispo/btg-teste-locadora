using Locadora.Application.Dtos;
using Locadora.Application.Interfaces;
using Locadora.Application.Mappings;
using Locadora.Domain.Repositories;

namespace Locadora.Application.Services;

public class PublicadoraService : IPublicadoraService
{
    private readonly IPublicadoraRepository _publicadoras;

    public PublicadoraService(IPublicadoraRepository publicadoras)
    {
        _publicadoras = publicadoras;
    }

    public async Task<IEnumerable<PublicadoraDto>> BuscarPorNomeAsync(string termo)
    {
        var publicadoras = await _publicadoras.BuscarPorNomeAsync(termo);
        return publicadoras.Select(p => p.ToDto()).ToList();
    }
}
