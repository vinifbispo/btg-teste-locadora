using Locadora.Application.Dtos;
using Locadora.Application.Mappings;
using Locadora.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Locadora.Application.Queries.Jogos.ListarJogos;

public class ListarJogosQueryHandler : IRequestHandler<ListarJogosQuery, PagedResultDto<JogoDto>>
{
    private const int TamanhoPaginaMaximo = 100;

    private readonly IJogoQueryRepository _jogos;
    private readonly ILogger<ListarJogosQueryHandler> _logger;

    public ListarJogosQueryHandler(IJogoQueryRepository jogos, ILogger<ListarJogosQueryHandler> logger)
    {
        _jogos = jogos;
        _logger = logger;
    }

    public async Task<PagedResultDto<JogoDto>> Handle(ListarJogosQuery request, CancellationToken cancellationToken)
    {
        var page = Math.Max(request.Page, 1);
        var pageSize = Math.Clamp(request.PageSize, 1, TamanhoPaginaMaximo);

        _logger.LogDebug("Listando jogos. Busca={Busca} Page={Page} PageSize={PageSize}", request.Busca, page, pageSize);

        var query = await _jogos.ListarAsync();

        if (!string.IsNullOrWhiteSpace(request.Busca))
            query = query.Where(j => j.Nome.Contains(request.Busca));

        query = query.OrderBy(j => j.Nome);

        var totalCount = await query.CountAsync(cancellationToken);
        var jogos = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);

        return new PagedResultDto<JogoDto>
        {
            Items = jogos.Select(j => j.ToDto()).ToList(),
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }
}
