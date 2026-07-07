using Locadora.Web.Domain.Models;

namespace Locadora.Web.Application.Interfaces;

public interface IGeneroService
{
    Task<IEnumerable<Genero>> BuscarPorNomeAsync(string busca, CancellationToken ct = default);
}
