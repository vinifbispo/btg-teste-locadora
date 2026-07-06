using Locadora.Application.Dtos;
using Locadora.Domain.Enums;

namespace Locadora.Application.Interfaces;

public interface IJogoService
{
    Task<IEnumerable<JogoDto>> ListarAsync(ConsoleTipo? console, string? busca);

    Task<JogoDto?> ObterPorIdAsync(int id);

    Task<JogoDto> CriarAsync(JogoInputDto input, string? chaveIdempotencia);

    Task<bool> AtualizarAsync(int id, JogoInputDto input);

    Task<bool> RemoverAsync(int id);
}
