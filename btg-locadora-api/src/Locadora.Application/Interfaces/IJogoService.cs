using Locadora.Application.Dtos;

namespace Locadora.Application.Interfaces;

public interface IJogoService
{
    Task<PagedResultDto<JogoDto>> ListarAsync(string? busca, int page = 1, int pageSize = 10);

    Task<JogoDto?> ObterPorIdAsync(int id);

    Task<JogoDto> CriarAsync(JogoInputDto input, string? chaveIdempotencia);

    Task<bool> AtualizarAsync(int id, JogoInputDto input);

    Task<bool> RemoverAsync(int id);

    Task ImportarDoJogoExternoAsync(CancellationToken ct = default);
}
