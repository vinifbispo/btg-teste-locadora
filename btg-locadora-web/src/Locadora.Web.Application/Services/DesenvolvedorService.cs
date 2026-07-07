using Locadora.Web.Application.Interfaces;
using Locadora.Web.Domain.Models;
using Microsoft.Extensions.Logging;

namespace Locadora.Web.Application.Services;

public class DesenvolvedorService : IDesenvolvedorService
{
    private readonly IDesenvolvedorApiClient _api;
    private readonly ILogger<DesenvolvedorService> _logger;

    public DesenvolvedorService(IDesenvolvedorApiClient api, ILogger<DesenvolvedorService> logger)
    {
        _api = api;
        _logger = logger;
    }

    public Task<IEnumerable<Desenvolvedor>> BuscarPorNomeAsync(string busca, CancellationToken ct = default)
    {
        _logger.LogDebug("Buscando desenvolvedores. Busca={Busca}", busca);
        return _api.BuscarPorNomeAsync(busca, ct);
    }
}
