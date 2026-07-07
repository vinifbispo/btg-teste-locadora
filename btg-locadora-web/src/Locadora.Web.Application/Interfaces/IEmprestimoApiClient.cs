using Locadora.Web.Domain.Models;

namespace Locadora.Web.Application.Interfaces;

public interface IEmprestimoApiClient
{
    Task<IEnumerable<Emprestimo>> ListarAsync(int? jogoId = null, int? amigoId = null, bool? apenasAtivos = null, CancellationToken ct = default);
    Task<Emprestimo?> ObterPorIdAsync(int id, CancellationToken ct = default);
    Task<Emprestimo> CriarAsync(EmprestimoInput input, string? chaveIdempotencia = null, CancellationToken ct = default);
    Task<bool> DevolverAsync(int id, CancellationToken ct = default);
}
