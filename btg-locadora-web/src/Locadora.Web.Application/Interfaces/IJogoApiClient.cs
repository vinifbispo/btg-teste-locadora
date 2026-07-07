using Locadora.Web.Domain.Models;

namespace Locadora.Web.Application.Interfaces;

public interface IJogoApiClient
{
    Task<PagedResult<Jogo>> ListarAsync(string? busca = null, int page = 1, int pageSize = 10, CancellationToken ct = default);
    Task<Jogo?> ObterPorIdAsync(int id, CancellationToken ct = default);
    Task<Jogo> CriarAsync(JogoInput input, string? chaveIdempotencia = null, CancellationToken ct = default);
    Task<bool> AtualizarAsync(int id, JogoInput input, CancellationToken ct = default);
    Task<bool> RemoverAsync(int id, CancellationToken ct = default);
}
