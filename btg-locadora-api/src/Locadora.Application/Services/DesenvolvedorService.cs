using Locadora.Application.Dtos;
using Locadora.Application.Interfaces;
using Locadora.Application.Mappings;
using Locadora.Domain.Repositories;

namespace Locadora.Application.Services;

public class DesenvolvedorService : IDesenvolvedorService
{
    private readonly IDesenvolvedorRepository _desenvolvedores;

    public DesenvolvedorService(IDesenvolvedorRepository desenvolvedores)
    {
        _desenvolvedores = desenvolvedores;
    }

    public async Task<IEnumerable<DesenvolvedorDto>> BuscarPorNomeAsync(string termo)
    {
        var desenvolvedores = await _desenvolvedores.BuscarPorNomeAsync(termo);
        return desenvolvedores.Select(d => d.ToDto()).ToList();
    }
}
