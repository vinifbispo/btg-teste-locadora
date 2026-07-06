using Locadora.Application.Dtos;

namespace Locadora.Application.Interfaces;

public interface IEmprestimoService
{
    Task<IEnumerable<EmprestimoDto>> ListarAsync(int? jogoId, int? amigoId, bool? apenasAtivos);

    Task<EmprestimoDto?> ObterPorIdAsync(int id);

    Task<EmprestimoDto> EmprestarAsync(EmprestimoInputDto input, string? chaveIdempotencia);

    Task<bool> DevolverAsync(int emprestimoId);
}
