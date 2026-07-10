using Locadora.Application.Dtos;
using Locadora.Application.Mappings;
using Locadora.Domain.Caching;
using Locadora.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Locadora.Application.Queries.Amigos.ObterAmigoPorId;

public class ObterAmigoPorIdQueryHandler : IRequestHandler<ObterAmigoPorIdQuery, AmigoDto?>
{
    private readonly IAmigoRepository _amigos;
    private readonly IAmigoCache _cache;
    private readonly ILogger<ObterAmigoPorIdQueryHandler> _logger;

    public ObterAmigoPorIdQueryHandler(IAmigoRepository amigos, IAmigoCache cache, ILogger<ObterAmigoPorIdQueryHandler> logger)
    {
        _amigos = amigos;
        _cache = cache;
        _logger = logger;
    }

    public async Task<AmigoDto?> Handle(ObterAmigoPorIdQuery request, CancellationToken cancellationToken)
    {
        var cacheado = await _cache.ObterAsync(request.Id);
        if (cacheado is not null)
        {
            _logger.LogDebug("Amigo {AmigoId} obtido do cache.", request.Id);
            return cacheado.ToDto();
        }

        var amigo = await _amigos.ObterPorIdAsync(request.Id);
        if (amigo is null)
        {
            _logger.LogWarning("Amigo {AmigoId} não encontrado.", request.Id);
            return null;
        }

        await _cache.DefinirAsync(amigo);
        return amigo.ToDto();
    }
}
