using Locadora.Web.Application.Interfaces;
using Locadora.Web.Domain.Models;
using Microsoft.Extensions.Logging;

namespace Locadora.Web.Application.Services;

public class AmigoService : IAmigoService
{
    private readonly IAmigoApiClient _api;
    private readonly ILogger<AmigoService> _logger;

    public AmigoService(IAmigoApiClient api, ILogger<AmigoService> logger)
    {
        _api = api;
        _logger = logger;
    }

    public Task<IEnumerable<Amigo>> ListarAsync(string? busca = null, CancellationToken ct = default)
    {
        _logger.LogDebug("Listando amigos. Busca={Busca}", busca);
        return _api.ListarAsync(busca, ct);
    }

    public Task<Amigo?> ObterPorIdAsync(int id, CancellationToken ct = default)
    {
        _logger.LogDebug("Obtendo amigo {AmigoId}.", id);
        return _api.ObterPorIdAsync(id, ct);
    }

    public async Task<Amigo> CriarAsync(AmigoInput input, string? chaveIdempotencia = null, CancellationToken ct = default)
    {
        var amigo = await _api.CriarAsync(input, chaveIdempotencia, ct);
        _logger.LogInformation("Amigo {AmigoId} criado.", amigo.Id);
        return amigo;
    }

    public async Task<bool> AtualizarAsync(int id, AmigoInput input, CancellationToken ct = default)
    {
        var sucesso = await _api.AtualizarAsync(id, input, ct);
        if (sucesso)
            _logger.LogInformation("Amigo {AmigoId} atualizado.", id);
        return sucesso;
    }

    public async Task<bool> RemoverAsync(int id, CancellationToken ct = default)
    {
        var sucesso = await _api.RemoverAsync(id, ct);
        if (sucesso)
            _logger.LogInformation("Amigo {AmigoId} removido.", id);
        return sucesso;
    }
}
