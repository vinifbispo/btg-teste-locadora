using Locadora.Application.Dtos;
using MediatR;

namespace Locadora.Application.Commands.Jogos.CriarJogo;

public record CriarJogoCommand(JogoInputDto Input, string? ChaveIdempotencia) : IRequest<JogoDto>;
