using System.Text.Json;
using Locadora.Domain.Caching;
using Locadora.Domain.Entities;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Locadora.Infrastructure.Caching;

public class EmprestimoRedisCache : IEmprestimoCache
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<EmprestimoRedisCache> _logger;
    private readonly DistributedCacheEntryOptions _opcoes;
    private readonly TimeSpan _tempo;

    public EmprestimoRedisCache(IDistributedCache cache, IOptions<CacheSettings> settings, ILogger<EmprestimoRedisCache> logger)
    {
        _cache = cache;
        _logger = logger;

        _tempo = TimeSpan.FromMilliseconds(settings.Value.TimeoutMilliseconds);

        _opcoes = new DistributedCacheEntryOptions();
        if (settings.Value.ExpirationMinutes > 0)
            _opcoes.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(settings.Value.ExpirationMinutes);
    }

    public async Task<Emprestimo?> ObterAsync(int id)
    {
        try
        {
            using var cts = new CancellationTokenSource(_tempo);
            var json = await _cache.GetStringAsync(Chave(id), cts.Token);
            return string.IsNullOrEmpty(json) ? null : JsonSerializer.Deserialize<Emprestimo>(json);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Falha ao ler o empréstimo {EmprestimoId} do cache; seguindo para o banco.", id);
            return null;
        }
    }

    public async Task DefinirAsync(Emprestimo emprestimo)
    {
        try
        {
            using var cts = new CancellationTokenSource(_tempo);
            await _cache.SetStringAsync(Chave(emprestimo.Id), JsonSerializer.Serialize(emprestimo), _opcoes, cts.Token);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Falha ao gravar o empréstimo {EmprestimoId} no cache; ignorando.", emprestimo.Id);
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
            _logger.LogWarning(ex, "Falha ao remover o empréstimo {EmprestimoId} do cache; ignorando.", id);
        }
    }

    private static string Chave(int id) => $"emprestimo:{id}";
}
