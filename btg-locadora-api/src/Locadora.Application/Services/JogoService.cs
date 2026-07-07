using Locadora.Application.Dtos;
using Locadora.Application.Idempotencia;
using Locadora.Application.Interfaces;
using Locadora.Application.Mappings;
using Locadora.Domain.Caching;
using Locadora.Domain.Entities;
using Locadora.Domain.Idempotencia;
using Locadora.Domain.Integracoes;
using Locadora.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Locadora.Application.Services;

public class JogoService : IJogoService
{
    private readonly IJogoRepository _jogos;
    private readonly IJogoCache _cache;
    private readonly IArmazenamentoIdempotencia _idempotencia;
    private readonly IJogoExternoApiClient _jogoExterno;
    private readonly ILogger<JogoService> _logger;

    public JogoService(
        IJogoRepository jogos,
        IJogoCache cache,
        IArmazenamentoIdempotencia idempotencia,
        IJogoExternoApiClient jogoExterno,
        ILogger<JogoService> logger)
    {
        _jogos = jogos;
        _cache = cache;
        _idempotencia = idempotencia;
        _jogoExterno = jogoExterno;
        _logger = logger;
    }

    public async Task<IEnumerable<JogoDto>> ListarAsync(string? busca)
    {
        _logger.LogDebug("Listando jogos. Busca={Busca}", busca);

        var query = await _jogos.ListarAsync();

        if (!string.IsNullOrWhiteSpace(busca))
            query = query.Where(j => j.Nome.Contains(busca));

        var jogos = await query.OrderBy(j => j.Nome).ToListAsync();
        return jogos.Select(j => j.ToDto()).ToList();
    }

    public async Task<JogoDto?> ObterPorIdAsync(int id)
    {
        var cacheado = await _cache.ObterAsync(id);
        if (cacheado is not null)
        {
            _logger.LogDebug("Jogo {JogoId} obtido do cache.", id);
            return cacheado.ToDto();
        }

        var jogo = await _jogos.ObterPorIdAsync(id);
        if (jogo is null)
        {
            _logger.LogWarning("Jogo {JogoId} não encontrado.", id);
            return null;
        }

        await _cache.DefinirAsync(jogo);
        return jogo.ToDto();
    }

    public async Task<JogoDto> CriarAsync(JogoInputDto input, string? chaveIdempotencia)
    {
        var existente = await ExecutorIdempotente.ObterAsync<JogoDto>(_idempotencia, "jogo:criar", chaveIdempotencia);
        if (existente is not null)
        {
            _logger.LogInformation("Criação de jogo idempotente: retornando resultado já existente para a chave {ChaveIdempotencia}.", chaveIdempotencia);
            return existente;
        }

        var jogo = new Jogo
        {
            Nome = input.Nome,
            Generos = input.Generos,
            Desenvolvedores = input.Desenvolvedores,
            Publicadoras = input.Publicadoras,
            DatasLancamento = input.DatasLancamento
                .Select(d => new DataLancamento { Regiao = d.Regiao, Data = d.Data })
                .ToList()
        };

        await _jogos.AdicionarAsync(jogo);
        await _cache.DefinirAsync(jogo);

        _logger.LogInformation("Jogo {JogoId} criado: {Nome}.", jogo.Id, jogo.Nome);

        var dto = jogo.ToDto();
        await ExecutorIdempotente.SalvarAsync(_idempotencia, "jogo:criar", chaveIdempotencia, dto);
        return dto;
    }

    public async Task<bool> AtualizarAsync(int id, JogoInputDto input)
    {
        var jogo = await _jogos.ObterPorIdAsync(id);
        if (jogo is null)
        {
            _logger.LogWarning("Atualização falhou: jogo {JogoId} não encontrado.", id);
            return false;
        }

        jogo.Nome = input.Nome;
        jogo.Generos = input.Generos;
        jogo.Desenvolvedores = input.Desenvolvedores;
        jogo.Publicadoras = input.Publicadoras;

        jogo.DatasLancamento.Clear();
        jogo.DatasLancamento.AddRange(input.DatasLancamento.Select(d => new DataLancamento { Regiao = d.Regiao, Data = d.Data }));

        await _jogos.AtualizarAsync(jogo);
        await _cache.DefinirAsync(jogo);

        _logger.LogInformation("Jogo {JogoId} atualizado.", id);
        return true;
    }

    public async Task<bool> RemoverAsync(int id)
    {
        var jogo = await _jogos.ObterPorIdAsync(id);
        if (jogo is null)
        {
            _logger.LogWarning("Remoção falhou: jogo {JogoId} não encontrado.", id);
            return false;
        }

        await _jogos.RemoverAsync(jogo);
        await _cache.RemoverAsync(id);

        _logger.LogInformation("Jogo {JogoId} removido.", id);
        return true;
    }

    public async Task ImportarDoJogoExternoAsync(CancellationToken ct = default)
    {
        var query = await _jogos.ListarAsync();
        if (await query.AnyAsync(ct))
        {
            _logger.LogInformation("Importação de jogos externos ignorada: já existem jogos cadastrados.");
            return;
        }

        _logger.LogInformation("Importação de jogos externos iniciada.");

        var externos = await _jogoExterno.ListarAsync(ct);

        var jogos = externos.Select(externo => new Jogo
        {
            Nome = externo.Nome,
            Generos = externo.Generos,
            Desenvolvedores = externo.Desenvolvedores,
            Publicadoras = externo.Publicadoras,
            DatasLancamento = externo.DatasLancamento
                .Select(kv => new DataLancamento { Regiao = kv.Key, Data = kv.Value })
                .ToList()
        }).ToList();

        await _jogos.AdicionarVariosAsync(jogos);

        foreach (var jogo in jogos)
            await _cache.DefinirAsync(jogo);

        _logger.LogInformation("Importação de jogos externos concluída: {Quantidade} jogos importados.", jogos.Count);
    }
}
