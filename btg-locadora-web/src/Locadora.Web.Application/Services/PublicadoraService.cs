using Locadora.Web.Application.Interfaces;
using Locadora.Web.Domain.Models;
using Microsoft.Extensions.Logging;

namespace Locadora.Web.Application.Services;

public class PublicadoraService : IPublicadoraService
{
    private readonly IPublicadoraApiClient _api;
    private readonly ILogger<PublicadoraService> _logger;

    public PublicadoraService(IPublicadoraApiClient api, ILogger<PublicadoraService> logger)
    {
        _api = api;
        _logger = logger;
    }

    public Task<IEnumerable<Publicadora>> BuscarPorNomeAsync(string busca, CancellationToken ct = default)
    {
        _logger.LogDebug("Buscando publicadoras. Busca={Busca}", busca);
        return _api.BuscarPorNomeAsync(busca, ct);
    }
}
