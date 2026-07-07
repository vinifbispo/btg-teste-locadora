using Locadora.Web.Application.Interfaces;
using Locadora.Web.Domain.Models;
using Microsoft.Extensions.Logging;

namespace Locadora.Web.Application.Services;

public class GeneroService : IGeneroService
{
    private readonly IGeneroApiClient _api;
    private readonly ILogger<GeneroService> _logger;

    public GeneroService(IGeneroApiClient api, ILogger<GeneroService> logger)
    {
        _api = api;
        _logger = logger;
    }

    public Task<IEnumerable<Genero>> BuscarPorNomeAsync(string busca, CancellationToken ct = default)
    {
        _logger.LogDebug("Buscando gêneros. Busca={Busca}", busca);
        return _api.BuscarPorNomeAsync(busca, ct);
    }
}
