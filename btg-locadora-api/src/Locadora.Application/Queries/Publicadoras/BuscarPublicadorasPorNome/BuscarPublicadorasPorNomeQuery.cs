using Locadora.Application.Dtos;
using MediatR;

namespace Locadora.Application.Queries.Publicadoras.BuscarPublicadorasPorNome;

public record BuscarPublicadorasPorNomeQuery(string Termo) : IRequest<IEnumerable<PublicadoraDto>>;
