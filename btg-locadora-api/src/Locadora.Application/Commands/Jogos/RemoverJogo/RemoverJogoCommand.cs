using MediatR;

namespace Locadora.Application.Commands.Jogos.RemoverJogo;

public record RemoverJogoCommand(int Id) : IRequest<bool>;
