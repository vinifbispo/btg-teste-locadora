using Locadora.Application.Dtos;
using MediatR;

namespace Locadora.Application.Queries.Jogos.ObterJogoPorId;

public record ObterJogoPorIdQuery(int Id) : IRequest<JogoDto?>;
