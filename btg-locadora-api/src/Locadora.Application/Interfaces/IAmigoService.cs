using Locadora.Application.Dtos;

namespace Locadora.Application.Interfaces;

public interface IAmigoService
{
    Task<PagedResultDto<AmigoDto>> ListarAsync(string? busca, int page = 1, int pageSize = 10);

    Task<AmigoDto?> ObterPorIdAsync(int id);

    Task<AmigoDto> CriarAsync(AmigoInputDto input, string? chaveIdempotencia);

    Task<bool> AtualizarAsync(int id, AmigoInputDto input);

    Task<bool> RemoverAsync(int id);
}
