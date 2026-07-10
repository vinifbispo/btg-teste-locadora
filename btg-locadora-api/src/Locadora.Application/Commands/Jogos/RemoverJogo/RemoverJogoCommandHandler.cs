using Locadora.Domain.Caching;
using Locadora.Domain.Exceptions;
using Locadora.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Locadora.Application.Commands.Jogos.RemoverJogo;

public class RemoverJogoCommandHandler : IRequestHandler<RemoverJogoCommand, bool>
{
    private readonly IJogoRepository _jogos;
    private readonly IEmprestimoRepository _emprestimos;
    private readonly IJogoCache _cache;
    private readonly ILogger<RemoverJogoCommandHandler> _logger;

    public RemoverJogoCommandHandler(
        IJogoRepository jogos,
        IEmprestimoRepository emprestimos,
        IJogoCache cache,
        ILogger<RemoverJogoCommandHandler> logger)
    {
        _jogos = jogos;
        _emprestimos = emprestimos;
        _cache = cache;
        _logger = logger;
    }

    public async Task<bool> Handle(RemoverJogoCommand request, CancellationToken cancellationToken)
    {
        var jogo = await _jogos.ObterPorIdAsync(request.Id);
        if (jogo is null)
        {
            _logger.LogWarning("Remoção falhou: jogo {JogoId} não encontrado.", request.Id);
            return false;
        }

        if (await _emprestimos.ExisteParaJogoAsync(request.Id))
        {
            _logger.LogWarning("Remoção falhou: jogo {JogoId} possui empréstimos associados.", request.Id);
            throw new ConflitoException("Não é possível remover o jogo pois ele possui empréstimos associados.");
        }

        await _jogos.RemoverAsync(jogo);
        await _cache.RemoverAsync(request.Id);

        _logger.LogInformation("Jogo {JogoId} removido.", request.Id);
        return true;
    }
}
