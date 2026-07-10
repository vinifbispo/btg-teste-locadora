using Locadora.Application.Dtos;
using Locadora.Application.Mappings;
using Locadora.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Locadora.Application.Queries.Emprestimos.ListarEmprestimos;

public class ListarEmprestimosQueryHandler : IRequestHandler<ListarEmprestimosQuery, PagedResultDto<EmprestimoDto>>
{
    private const int TamanhoPaginaMaximo = 100;

    private readonly IEmprestimoRepository _emprestimos;
    private readonly ILogger<ListarEmprestimosQueryHandler> _logger;

    public ListarEmprestimosQueryHandler(IEmprestimoRepository emprestimos, ILogger<ListarEmprestimosQueryHandler> logger)
    {
        _emprestimos = emprestimos;
        _logger = logger;
    }

    public async Task<PagedResultDto<EmprestimoDto>> Handle(ListarEmprestimosQuery request, CancellationToken cancellationToken)
    {
        var page = Math.Max(request.Page, 1);
        var pageSize = Math.Clamp(request.PageSize, 1, TamanhoPaginaMaximo);

        _logger.LogDebug(
            "Listando empréstimos. JogoId={JogoId} AmigoId={AmigoId} ApenasAtivos={ApenasAtivos} Page={Page} PageSize={PageSize}",
            request.JogoId, request.AmigoId, request.ApenasAtivos, page, pageSize);

        var query = await _emprestimos.ListarAsync();

        if (request.JogoId is not null)
            query = query.Where(e => e.JogoId == request.JogoId);

        if (request.AmigoId is not null)
            query = query.Where(e => e.AmigoId == request.AmigoId);

        if (request.ApenasAtivos is true)
            query = query.Where(e => e.DataDevolucao == null);

        query = query.OrderByDescending(e => e.DataEmprestimo);

        var totalCount = await query.CountAsync(cancellationToken);
        var emprestimos = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);

        return new PagedResultDto<EmprestimoDto>
        {
            Items = emprestimos.Select(e => e.ToDto()).ToList(),
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }
}
