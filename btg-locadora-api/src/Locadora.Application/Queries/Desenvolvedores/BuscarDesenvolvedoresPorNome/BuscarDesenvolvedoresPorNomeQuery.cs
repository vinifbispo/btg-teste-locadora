using Locadora.Application.Dtos;
using MediatR;

namespace Locadora.Application.Queries.Desenvolvedores.BuscarDesenvolvedoresPorNome;

public record BuscarDesenvolvedoresPorNomeQuery(string Termo) : IRequest<IEnumerable<DesenvolvedorDto>>;
