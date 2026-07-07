using Locadora.Application.Dtos;

namespace Locadora.Application.Interfaces;

public interface IEmprestimoService
{
    Task<PagedResultDto<EmprestimoDto>> ListarAsync(int? jogoId, int? amigoId, bool? apenasAtivos, int page = 1, int pageSize = 10);

    Task<EmprestimoDto?> ObterPorIdAsync(int id);

    Task<EmprestimoDto> EmprestarAsync(EmprestimoInputDto input, string? chaveIdempotencia);

    Task<bool> DevolverAsync(int emprestimoId);
}
