using Locadora.Domain.Integracoes.Dtos;

namespace Locadora.Domain.Integracoes;

public interface IJogoExternoApiClient
{
    Task<IEnumerable<JogoExterno>> ListarAsync(CancellationToken ct = default);
}
