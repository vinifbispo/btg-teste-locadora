using Locadora.Web.Domain.Models;

namespace Locadora.Web.Application.Interfaces;

public interface IDesenvolvedorApiClient
{
    Task<IEnumerable<Desenvolvedor>> BuscarPorNomeAsync(string busca, CancellationToken ct = default);
}
