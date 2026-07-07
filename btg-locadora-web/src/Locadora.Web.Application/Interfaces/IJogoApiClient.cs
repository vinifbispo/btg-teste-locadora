using Locadora.Web.Domain.Models;

namespace Locadora.Web.Application.Interfaces;

public interface IJogoApiClient
{
    Task<IEnumerable<Jogo>> ListarAsync(string? busca = null, CancellationToken ct = default);
    Task<Jogo?> ObterPorIdAsync(int id, CancellationToken ct = default);
    Task<Jogo> CriarAsync(JogoInput input, string? chaveIdempotencia = null, CancellationToken ct = default);
    Task<bool> AtualizarAsync(int id, JogoInput input, CancellationToken ct = default);
    Task<bool> RemoverAsync(int id, CancellationToken ct = default);
}
