using Locadora.Application.Dtos;
using Locadora.Application.Idempotencia;
using Locadora.Application.Interfaces;
using Locadora.Application.Mappings;
using Locadora.Domain.Caching;
using Locadora.Domain.Entities;
using Locadora.Domain.Exceptions;
using Locadora.Domain.Idempotencia;
using Locadora.Domain.Integracoes;
using Locadora.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Locadora.Application.Services;

public class JogoService : IJogoService
{
    private const int TamanhoPaginaMaximo = 100;

    private readonly IJogoRepository _jogos;
    private readonly IGeneroRepository _generos;
    private readonly IDesenvolvedorRepository _desenvolvedores;
    private readonly IPublicadoraRepository _publicadoras;
    private readonly IJogoCache _cache;
    private readonly IArmazenamentoIdempotencia _idempotencia;
    private readonly IJogoExternoApiClient _jogoExterno;
    private readonly ILogger<JogoService> _logger;

    public JogoService(
        IJogoRepository jogos,
        IGeneroRepository generos,
        IDesenvolvedorRepository desenvolvedores,
        IPublicadoraRepository publicadoras,
        IJogoCache cache,
        IArmazenamentoIdempotencia idempotencia,
        IJogoExternoApiClient jogoExterno,
        ILogger<JogoService> logger)
    {
        _jogos = jogos;
        _generos = generos;
        _desenvolvedores = desenvolvedores;
        _publicadoras = publicadoras;
        _cache = cache;
        _idempotencia = idempotencia;
        _jogoExterno = jogoExterno;
        _logger = logger;
    }

    public async Task<PagedResultDto<JogoDto>> ListarAsync(string? busca, int page = 1, int pageSize = 10)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, TamanhoPaginaMaximo);

        _logger.LogDebug("Listando jogos. Busca={Busca} Page={Page} PageSize={PageSize}", busca, page, pageSize);

        var query = await _jogos.ListarAsync();

        if (!string.IsNullOrWhiteSpace(busca))
            query = query.Where(j => j.Nome.Contains(busca));

        query = query.OrderBy(j => j.Nome);

        var totalCount = await query.CountAsync();
        var jogos = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PagedResultDto<JogoDto>
        {
            Items = jogos.Select(j => j.ToDto()).ToList(),
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
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
            Generos = await ResolverGenerosAsync(input.GeneroIds),
            Desenvolvedores = await ResolverDesenvolvedoresAsync(input.DesenvolvedorIds),
            Publicadoras = await ResolverPublicadorasAsync(input.PublicadoraIds),
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
        jogo.Generos = await ResolverGenerosAsync(input.GeneroIds);
        jogo.Desenvolvedores = await ResolverDesenvolvedoresAsync(input.DesenvolvedorIds);
        jogo.Publicadoras = await ResolverPublicadorasAsync(input.PublicadoraIds);

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

        var generos = await _generos.ObterOuCriarPorNomesAsync(externos.SelectMany(e => e.Generos));
        var desenvolvedores = await _desenvolvedores.ObterOuCriarPorNomesAsync(externos.SelectMany(e => e.Desenvolvedores));
        var publicadoras = await _publicadoras.ObterOuCriarPorNomesAsync(externos.SelectMany(e => e.Publicadoras));

        var jogos = externos.Select(externo => new Jogo
        {
            Nome = externo.Nome,
            Generos = generos.Where(g => externo.Generos.Contains(g.Nome, StringComparer.OrdinalIgnoreCase)).ToList(),
            Desenvolvedores = desenvolvedores.Where(d => externo.Desenvolvedores.Contains(d.Nome, StringComparer.OrdinalIgnoreCase)).ToList(),
            Publicadoras = publicadoras.Where(p => externo.Publicadoras.Contains(p.Nome, StringComparer.OrdinalIgnoreCase)).ToList(),
            DatasLancamento = externo.DatasLancamento
                .Select(kv => new DataLancamento { Regiao = kv.Key, Data = kv.Value })
                .ToList()
        }).ToList();

        await _jogos.AdicionarVariosAsync(jogos);

        foreach (var jogo in jogos)
            await _cache.DefinirAsync(jogo);

        _logger.LogInformation("Importação de jogos externos concluída: {Quantidade} jogos importados.", jogos.Count);
    }

    private async Task<List<Genero>> ResolverGenerosAsync(List<int> ids)
    {
        var generos = await _generos.ListarPorIdsAsync(ids);
        var faltantes = ids.Except(generos.Select(g => g.Id)).ToList();
        if (faltantes.Count > 0)
            throw new NotFoundException($"Gênero(s) não encontrado(s): {string.Join(", ", faltantes)}.");

        return generos;
    }

    private async Task<List<Desenvolvedor>> ResolverDesenvolvedoresAsync(List<int> ids)
    {
        var desenvolvedores = await _desenvolvedores.ListarPorIdsAsync(ids);
        var faltantes = ids.Except(desenvolvedores.Select(d => d.Id)).ToList();
        if (faltantes.Count > 0)
            throw new NotFoundException($"Desenvolvedor(es) não encontrado(s): {string.Join(", ", faltantes)}.");

        return desenvolvedores;
    }

    private async Task<List<Publicadora>> ResolverPublicadorasAsync(List<int> ids)
    {
        var publicadoras = await _publicadoras.ListarPorIdsAsync(ids);
        var faltantes = ids.Except(publicadoras.Select(p => p.Id)).ToList();
        if (faltantes.Count > 0)
            throw new NotFoundException($"Publicadora(s) não encontrada(s): {string.Join(", ", faltantes)}.");

        return publicadoras;
    }
}
