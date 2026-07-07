using Locadora.Web.Domain.Models;

namespace Locadora.Web.Application.Interfaces;

public interface IGeneroApiClient
{
    Task<IEnumerable<Genero>> BuscarPorNomeAsync(string busca, CancellationToken ct = default);
}
