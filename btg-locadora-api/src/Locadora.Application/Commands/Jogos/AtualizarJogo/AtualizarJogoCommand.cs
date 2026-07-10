using Locadora.Application.Dtos;
using MediatR;

namespace Locadora.Application.Commands.Jogos.AtualizarJogo;

public record AtualizarJogoCommand(int Id, JogoInputDto Input) : IRequest<bool>;
