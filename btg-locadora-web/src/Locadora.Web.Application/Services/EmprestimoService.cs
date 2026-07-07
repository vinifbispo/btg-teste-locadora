using Locadora.Web.Application.Interfaces;
using Locadora.Web.Domain.Models;
using Microsoft.Extensions.Logging;

namespace Locadora.Web.Application.Services;

public class EmprestimoService : IEmprestimoService
{
    private readonly IEmprestimoApiClient _api;
    private readonly ILogger<EmprestimoService> _logger;

    public EmprestimoService(IEmprestimoApiClient api, ILogger<EmprestimoService> logger)
    {
        _api = api;
        _logger = logger;
    }

    public Task<PagedResult<Emprestimo>> ListarAsync(int? jogoId = null, int? amigoId = null, bool? apenasAtivos = null, int page = 1, int pageSize = 10, CancellationToken ct = default)
    {
        _logger.LogDebug("Listando empréstimos. JogoId={JogoId}, AmigoId={AmigoId}, ApenasAtivos={ApenasAtivos}, Page={Page}", jogoId, amigoId, apenasAtivos, page);
        return _api.ListarAsync(jogoId, amigoId, apenasAtivos, page, pageSize, ct);
    }

    public Task<Emprestimo?> ObterPorIdAsync(int id, CancellationToken ct = default)
    {
        _logger.LogDebug("Obtendo empréstimo {EmprestimoId}.", id);
        return _api.ObterPorIdAsync(id, ct);
    }

    public async Task<Emprestimo> CriarAsync(EmprestimoInput input, string? chaveIdempotencia = null, CancellationToken ct = default)
    {
        var emprestimo = await _api.CriarAsync(input, chaveIdempotencia, ct);
        _logger.LogInformation("Empréstimo {EmprestimoId} criado.", emprestimo.Id);
        return emprestimo;
    }

    public async Task<bool> DevolverAsync(int id, CancellationToken ct = default)
    {
        var sucesso = await _api.DevolverAsync(id, ct);
        if (sucesso)
            _logger.LogInformation("Empréstimo {EmprestimoId} devolvido.", id);
        return sucesso;
    }
}
