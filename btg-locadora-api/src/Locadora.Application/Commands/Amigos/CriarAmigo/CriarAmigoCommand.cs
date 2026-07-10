using Locadora.Application.Dtos;
using MediatR;

namespace Locadora.Application.Commands.Amigos.CriarAmigo;

public record CriarAmigoCommand(AmigoInputDto Input, string? ChaveIdempotencia) : IRequest<AmigoDto>;
