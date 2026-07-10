using Locadora.Application.Dtos;
using MediatR;

namespace Locadora.Application.Queries.Generos.BuscarGenerosPorNome;

public record BuscarGenerosPorNomeQuery(string Termo) : IRequest<IEnumerable<GeneroDto>>;
