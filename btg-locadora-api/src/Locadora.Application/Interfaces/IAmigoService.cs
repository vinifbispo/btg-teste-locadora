using Locadora.Application.Dtos;

namespace Locadora.Application.Interfaces;

public interface IAmigoService
{
    Task<IEnumerable<AmigoDto>> ListarAsync(string? busca);

    Task<AmigoDto?> ObterPorIdAsync(int id);

    Task<AmigoDto> CriarAsync(AmigoInputDto input, string? chaveIdempotencia);

    Task<bool> AtualizarAsync(int id, AmigoInputDto input);

    Task<bool> RemoverAsync(int id);
}
