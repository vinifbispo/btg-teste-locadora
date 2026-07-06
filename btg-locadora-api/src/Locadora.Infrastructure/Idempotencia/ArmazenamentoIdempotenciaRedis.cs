using Locadora.Domain.Idempotencia;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Locadora.Infrastructure.Idempotencia;

public class ArmazenamentoIdempotenciaRedis : IArmazenamentoIdempotencia
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<ArmazenamentoIdempotenciaRedis> _logger;
    private readonly DistributedCacheEntryOptions _opcoes;

    public ArmazenamentoIdempotenciaRedis(IDistributedCache cache, IOptions<IdempotenciaSettings> settings, ILogger<ArmazenamentoIdempotenciaRedis> logger)
    {
        _cache = cache;
        _logger = logger;

        _opcoes = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(settings.Value.ExpirationHours)
        };
    }

    public async Task<string?> ObterAsync(string chave)
    {
        try
        {
            return await _cache.GetStringAsync(Chave(chave));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Falha ao ler a chave de idempotência {Chave} do Redis; seguindo sem cache.", chave);
            return null;
        }
    }

    public async Task ArmazenarAsync(string chave, string valor)
    {
        try
        {
            await _cache.SetStringAsync(Chave(chave), valor, _opcoes);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Falha ao gravar a chave de idempotência {Chave} no Redis; ignorando.", chave);
        }
    }

    private static string Chave(string chave) => $"idempotencia:{chave}";
}
