using Locadora.Application.Dtos;
using Locadora.Application.Mappings;
using Locadora.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Locadora.Application.Queries.Amigos.ListarAmigos;

public class ListarAmigosQueryHandler : IRequestHandler<ListarAmigosQuery, PagedResultDto<AmigoDto>>
{
    private const int TamanhoPaginaMaximo = 100;

    private readonly IAmigoRepository _amigos;
    private readonly ILogger<ListarAmigosQueryHandler> _logger;

    public ListarAmigosQueryHandler(IAmigoRepository amigos, ILogger<ListarAmigosQueryHandler> logger)
    {
        _amigos = amigos;
        _logger = logger;
    }

    public async Task<PagedResultDto<AmigoDto>> Handle(ListarAmigosQuery request, CancellationToken cancellationToken)
    {
        var page = Math.Max(request.Page, 1);
        var pageSize = Math.Clamp(request.PageSize, 1, TamanhoPaginaMaximo);

        _logger.LogDebug("Listando amigos. Busca={Busca} Page={Page} PageSize={PageSize}", request.Busca, page, pageSize);

        var query = await _amigos.ListarAsync();

        if (!string.IsNullOrWhiteSpace(request.Busca))
            query = query.Where(a => a.Nome.Contains(request.Busca) || a.Sobrenome.Contains(request.Busca));

        query = query.OrderBy(a => a.Nome);

        var totalCount = await query.CountAsync(cancellationToken);
        var amigos = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);

        return new PagedResultDto<AmigoDto>
        {
            Items = amigos.Select(a => a.ToDto()).ToList(),
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }
}
