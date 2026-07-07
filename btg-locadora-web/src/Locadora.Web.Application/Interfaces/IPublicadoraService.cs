using Locadora.Web.Domain.Models;

namespace Locadora.Web.Application.Interfaces;

public interface IPublicadoraService
{
    Task<IEnumerable<Publicadora>> BuscarPorNomeAsync(string busca, CancellationToken ct = default);
}
