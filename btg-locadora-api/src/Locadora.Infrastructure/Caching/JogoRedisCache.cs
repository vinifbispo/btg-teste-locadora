using System.Text.Json;
using Locadora.Domain.Caching;
using Locadora.Domain.Entities;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Locadora.Infrastructure.Caching;

public class JogoRedisCache : IJogoCache
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<JogoRedisCache> _logger;
    private readonly DistributedCacheEntryOptions _opcoes;
    private readonly TimeSpan _tempo;

    public JogoRedisCache(IDistributedCache cache, IOptions<CacheSettings> settings, ILogger<JogoRedisCache> logger)
    {
        _cache = cache;
        _logger = logger;

        _tempo = TimeSpan.FromMilliseconds(settings.Value.TimeoutMilliseconds);

        _opcoes = new DistributedCacheEntryOptions();
        if (settings.Value.ExpirationMinutes > 0)
            _opcoes.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(settings.Value.ExpirationMinutes);
    }

    public async Task<Jogo?> ObterAsync(int id)
    {
        try
        {
            using var cts = new CancellationTokenSource(_tempo);
            var json = await _cache.GetStringAsync(Chave(id), cts.Token);
            return string.IsNullOrEmpty(json) ? null : JsonSerializer.Deserialize<Jogo>(json);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Falha ao ler o jogo {JogoId} do cache; seguindo para o banco.", id);
            return null;
        }
    }

    public async Task DefinirAsync(Jogo jogo)
    {
        try
        {
            using var cts = new CancellationTokenSource(_tempo);
            await _cache.SetStringAsync(Chave(jogo.Id), JsonSerializer.Serialize(jogo), _opcoes, cts.Token);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Falha ao gravar o jogo {JogoId} no cache; ignorando.", jogo.Id);
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
            _logger.LogWarning(ex, "Falha ao remover o jogo {JogoId} do cache; ignorando.", id);
        }
    }

    private static string Chave(int id) => $"jogo:{id}";
}
