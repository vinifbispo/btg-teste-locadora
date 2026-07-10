using MediatR;

namespace Locadora.Application.Commands.Amigos.RemoverAmigo;

public record RemoverAmigoCommand(int Id) : IRequest<bool>;
