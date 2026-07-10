using Locadora.Domain.Caching;
using Locadora.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Locadora.Application.Commands.Emprestimos.DevolverEmprestimo;

public class DevolverEmprestimoCommandHandler : IRequestHandler<DevolverEmprestimoCommand, bool>
{
    private readonly IEmprestimoRepository _emprestimos;
    private readonly IEmprestimoCache _cache;
    private readonly ILogger<DevolverEmprestimoCommandHandler> _logger;

    public DevolverEmprestimoCommandHandler(IEmprestimoRepository emprestimos, IEmprestimoCache cache, ILogger<DevolverEmprestimoCommandHandler> logger)
    {
        _emprestimos = emprestimos;
        _cache = cache;
        _logger = logger;
    }

    public async Task<bool> Handle(DevolverEmprestimoCommand request, CancellationToken cancellationToken)
    {
        var emprestimo = await _emprestimos.ObterPorIdAsync(request.EmprestimoId);
        if (emprestimo is null || emprestimo.DataDevolucao is not null)
        {
            _logger.LogWarning("Devolução falhou: empréstimo {EmprestimoId} não encontrado ou já devolvido.", request.EmprestimoId);
            return false;
        }

        emprestimo.DataDevolucao = DateTime.UtcNow;
        await _emprestimos.AtualizarAsync(emprestimo);
        await _cache.DefinirAsync(emprestimo);

        _logger.LogInformation("Empréstimo {EmprestimoId} devolvido.", request.EmprestimoId);
        return true;
    }
}
