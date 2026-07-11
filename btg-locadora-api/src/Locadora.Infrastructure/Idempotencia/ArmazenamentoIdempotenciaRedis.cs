using Locadora.Domain.Idempotencia;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Locadora.Infrastructure.Idempotencia;

public class ArmazenamentoIdempotenciaRedis : IArmazenamentoIdempotencia
{
    private const string ValorReservado = "__reservado__";

    private readonly IConnectionMultiplexer _conexao;
    private readonly ILogger<ArmazenamentoIdempotenciaRedis> _logger;
    private readonly TimeSpan _tempoExpiracao;
    private readonly TimeSpan _tempoReserva;

    public ArmazenamentoIdempotenciaRedis(IConnectionMultiplexer conexao, IOptions<IdempotenciaSettings> settings, ILogger<ArmazenamentoIdempotenciaRedis> logger)
    {
        _conexao = conexao;
        _logger = logger;
        _tempoExpiracao = TimeSpan.FromHours(settings.Value.ExpirationHours);
        _tempoReserva = TimeSpan.FromSeconds(30);
    }

    public async Task<string?> ObterAsync(string chave)
    {
        try
        {
            var valor = await Db().StringGetAsync(Chave(chave));
            return valor.IsNullOrEmpty || valor == ValorReservado ? null : valor.ToString();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Falha ao ler a chave de idempotência {Chave} do Redis; seguindo sem cache.", chave);
            return null;
        }
    }

    public async Task<bool> TentarReservarAsync(string chave)
    {
        try
        {
            return await Db().StringSetAsync(Chave(chave), ValorReservado, _tempoReserva, When.NotExists);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Falha ao reservar a chave de idempotência {Chave} no Redis; seguindo sem lock.", chave);
            return true;
        }
    }

    public async Task ArmazenarAsync(string chave, string valor)
    {
        try
        {
            await Db().StringSetAsync(Chave(chave), valor, _tempoExpiracao);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Falha ao gravar a chave de idempotência {Chave} no Redis; ignorando.", chave);
        }
    }

    private IDatabase Db() => _conexao.GetDatabase();

    private static string Chave(string chave) => $"idempotencia:{chave}";
}
