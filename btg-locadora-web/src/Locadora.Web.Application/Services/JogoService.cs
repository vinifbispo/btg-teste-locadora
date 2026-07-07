using Locadora.Web.Application.Interfaces;
using Locadora.Web.Domain.Models;
using Microsoft.Extensions.Logging;

namespace Locadora.Web.Application.Services;

public class JogoService : IJogoService
{
    private readonly IJogoApiClient _api;
    private readonly ILogger<JogoService> _logger;

    public JogoService(IJogoApiClient api, ILogger<JogoService> logger)
    {
        _api = api;
        _logger = logger;
    }

    public Task<PagedResult<Jogo>> ListarAsync(string? busca = null, int page = 1, int pageSize = 10, CancellationToken ct = default)
    {
        _logger.LogDebug("Listando jogos. Busca={Busca} Page={Page}", busca, page);
        return _api.ListarAsync(busca, page, pageSize, ct);
    }

    public Task<Jogo?> ObterPorIdAsync(int id, CancellationToken ct = default)
    {
        _logger.LogDebug("Obtendo jogo {JogoId}.", id);
        return _api.ObterPorIdAsync(id, ct);
    }

    public async Task<Jogo> CriarAsync(JogoInput input, string? chaveIdempotencia = null, CancellationToken ct = default)
    {
        var jogo = await _api.CriarAsync(input, chaveIdempotencia, ct);
        _logger.LogInformation("Jogo {JogoId} criado.", jogo.Id);
        return jogo;
    }

    public async Task<bool> AtualizarAsync(int id, JogoInput input, CancellationToken ct = default)
    {
        var sucesso = await _api.AtualizarAsync(id, input, ct);
        if (sucesso)
            _logger.LogInformation("Jogo {JogoId} atualizado.", id);
        return sucesso;
    }

    public async Task<bool> RemoverAsync(int id, CancellationToken ct = default)
    {
        var sucesso = await _api.RemoverAsync(id, ct);
        if (sucesso)
            _logger.LogInformation("Jogo {JogoId} removido.", id);
        return sucesso;
    }
}
