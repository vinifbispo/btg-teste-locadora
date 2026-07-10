using Locadora.Application.Dtos;
using MediatR;

namespace Locadora.Application.Queries.Jogos.ListarJogos;

public record ListarJogosQuery(string? Busca, int Page = 1, int PageSize = 10) : IRequest<PagedResultDto<JogoDto>>;
