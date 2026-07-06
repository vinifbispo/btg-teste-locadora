using System.Text.Json;
using Locadora.Domain.Caching;
using Locadora.Domain.Entities;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Locadora.Infrastructure.Caching;

public class AmigoRedisCache : IAmigoCache
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<AmigoRedisCache> _logger;
    private readonly DistributedCacheEntryOptions _opcoes;
    private readonly TimeSpan _tempo;

    public AmigoRedisCache(IDistributedCache cache, IOptions<CacheSettings> settings, ILogger<AmigoRedisCache> logger)
    {
        _cache = cache;
        _logger = logger;

        _tempo = TimeSpan.FromMilliseconds(settings.Value.TimeoutMilliseconds);

        _opcoes = new DistributedCacheEntryOptions();
        if (settings.Value.ExpirationMinutes > 0)
            _opcoes.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(settings.Value.ExpirationMinutes);
    }

    public async Task<Amigo?> ObterAsync(int id)
    {
        try
        {
            using var cts = new CancellationTokenSource(_tempo);
            var json = await _cache.GetStringAsync(Chave(id), cts.Token);
            return string.IsNullOrEmpty(json) ? null : JsonSerializer.Deserialize<Amigo>(json);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Falha ao ler o amigo {AmigoId} do cache; seguindo para o banco.", id);
            return null;
        }
    }

    public async Task DefinirAsync(Amigo amigo)
    {
        try
        {
            using var cts = new CancellationTokenSource(_tempo);
            await _cache.SetStringAsync(Chave(amigo.Id), JsonSerializer.Serialize(amigo), _opcoes, cts.Token);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Falha ao gravar o amigo {AmigoId} no cache; ignorando.", amigo.Id);
        }
    }

    public async Task RemoverAsync(int id)
    {
        try
        {
            using var cts = new CancellationTokenSource(_tempo);
            await _cache.RemoveAsync(Chave(id), cts.Token);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Falha ao remover o amigo {AmigoId} do cache; ignorando.", id);
        }
    }

    private static string Chave(int id) => $"amigo:{id}";
}
