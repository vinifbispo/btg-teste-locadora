using Locadora.Application.Dtos;
using Locadora.Application.Mappings;
using Locadora.Domain.Caching;
using Locadora.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Locadora.Application.Queries.Emprestimos.ObterEmprestimoPorId;

public class ObterEmprestimoPorIdQueryHandler : IRequestHandler<ObterEmprestimoPorIdQuery, EmprestimoDto?>
{
    private readonly IEmprestimoRepository _emprestimos;
    private readonly IEmprestimoCache _cache;
    private readonly ILogger<ObterEmprestimoPorIdQueryHandler> _logger;

    public ObterEmprestimoPorIdQueryHandler(IEmprestimoRepository emprestimos, IEmprestimoCache cache, ILogger<ObterEmprestimoPorIdQueryHandler> logger)
    {
        _emprestimos = emprestimos;
        _cache = cache;
        _logger = logger;
    }

    public async Task<EmprestimoDto?> Handle(ObterEmprestimoPorIdQuery request, CancellationToken cancellationToken)
    {
        var cacheado = await _cache.ObterAsync(request.Id);
        if (cacheado is not null)
        {
            _logger.LogDebug("Empréstimo {EmprestimoId} obtido do cache.", request.Id);
            return cacheado.ToDto();
        }

        var emprestimo = await _emprestimos.ObterPorIdAsync(request.Id);
        if (emprestimo is null)
        {
            _logger.LogWarning("Empréstimo {EmprestimoId} não encontrado.", request.Id);
            return null;
        }

        await _cache.DefinirAsync(emprestimo);
        return emprestimo.ToDto();
    }
}
