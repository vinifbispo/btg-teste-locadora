using Locadora.Application.Dtos;
using Locadora.Application.Mappings;
using Locadora.Domain.Caching;
using Locadora.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Locadora.Application.Queries.Jogos.ObterJogoPorId;

public class ObterJogoPorIdQueryHandler : IRequestHandler<ObterJogoPorIdQuery, JogoDto?>
{
    private readonly IJogoRepository _jogos;
    private readonly IJogoCache _cache;
    private readonly ILogger<ObterJogoPorIdQueryHandler> _logger;

    public ObterJogoPorIdQueryHandler(IJogoRepository jogos, IJogoCache cache, ILogger<ObterJogoPorIdQueryHandler> logger)
    {
        _jogos = jogos;
        _cache = cache;
        _logger = logger;
    }

    public async Task<JogoDto?> Handle(ObterJogoPorIdQuery request, CancellationToken cancellationToken)
    {
        var cacheado = await _cache.ObterAsync(request.Id);
        if (cacheado is not null)
        {
            _logger.LogDebug("Jogo {JogoId} obtido do cache.", request.Id);
            return cacheado.ToDto();
        }

        var jogo = await _jogos.ObterPorIdAsync(request.Id);
        if (jogo is null)
        {
            _logger.LogWarning("Jogo {JogoId} não encontrado.", request.Id);
            return null;
        }

        await _cache.DefinirAsync(jogo);
        return jogo.ToDto();
    }
}
