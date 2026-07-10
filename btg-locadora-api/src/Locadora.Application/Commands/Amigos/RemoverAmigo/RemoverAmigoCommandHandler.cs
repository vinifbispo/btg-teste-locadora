using Locadora.Domain.Caching;
using Locadora.Domain.Exceptions;
using Locadora.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Locadora.Application.Commands.Amigos.RemoverAmigo;

public class RemoverAmigoCommandHandler : IRequestHandler<RemoverAmigoCommand, bool>
{
    private readonly IAmigoRepository _amigos;
    private readonly IEmprestimoRepository _emprestimos;
    private readonly IAmigoCache _cache;
    private readonly ILogger<RemoverAmigoCommandHandler> _logger;

    public RemoverAmigoCommandHandler(
        IAmigoRepository amigos,
        IEmprestimoRepository emprestimos,
        IAmigoCache cache,
        ILogger<RemoverAmigoCommandHandler> logger)
    {
        _amigos = amigos;
        _emprestimos = emprestimos;
        _cache = cache;
        _logger = logger;
    }

    public async Task<bool> Handle(RemoverAmigoCommand request, CancellationToken cancellationToken)
    {
        var amigo = await _amigos.ObterPorIdAsync(request.Id);
        if (amigo is null)
        {
            _logger.LogWarning("Remoção falhou: amigo {AmigoId} não encontrado.", request.Id);
            return false;
        }

        if (await _emprestimos.ExisteParaAmigoAsync(request.Id))
        {
            _logger.LogWarning("Remoção falhou: amigo {AmigoId} possui empréstimos associados.", request.Id);
            throw new ConflitoException("Não é possível remover o amigo pois ele possui empréstimos associados.");
        }

        await _amigos.RemoverAsync(amigo);
        await _cache.RemoverAsync(request.Id);

        _logger.LogInformation("Amigo {AmigoId} removido.", request.Id);
        return true;
    }
}
