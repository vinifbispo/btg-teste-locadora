using Locadora.Web.Domain.Models;

namespace Locadora.Web.Application.Interfaces;

public interface IAmigoApiClient
{
    Task<IEnumerable<Amigo>> ListarAsync(string? busca = null, CancellationToken ct = default);
    Task<Amigo?> ObterPorIdAsync(int id, CancellationToken ct = default);
    Task<Amigo> CriarAsync(AmigoInput input, string? chaveIdempotencia = null, CancellationToken ct = default);
    Task<bool> AtualizarAsync(int id, AmigoInput input, CancellationToken ct = default);
    Task<bool> RemoverAsync(int id, CancellationToken ct = default);
}
