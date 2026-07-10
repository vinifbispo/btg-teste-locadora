using Locadora.Application.Dtos;
using MediatR;

namespace Locadora.Application.Queries.Amigos.ListarAmigos;

public record ListarAmigosQuery(string? Busca, int Page = 1, int PageSize = 10) : IRequest<PagedResultDto<AmigoDto>>;
