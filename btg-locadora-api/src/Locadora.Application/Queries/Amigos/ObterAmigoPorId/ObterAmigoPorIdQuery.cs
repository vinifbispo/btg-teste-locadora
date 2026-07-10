using Locadora.Application.Dtos;
using MediatR;

namespace Locadora.Application.Queries.Amigos.ObterAmigoPorId;

public record ObterAmigoPorIdQuery(int Id) : IRequest<AmigoDto?>;
