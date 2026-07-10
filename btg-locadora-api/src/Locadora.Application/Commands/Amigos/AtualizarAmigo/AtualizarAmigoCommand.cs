using Locadora.Application.Dtos;
using MediatR;

namespace Locadora.Application.Commands.Amigos.AtualizarAmigo;

public record AtualizarAmigoCommand(int Id, AmigoInputDto Input) : IRequest<bool>;
