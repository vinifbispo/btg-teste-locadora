using Locadora.Application.Dtos;
using Locadora.Application.Idempotencia;
using Locadora.Application.Interfaces;
using Locadora.Application.Mappings;
using Locadora.Domain.Caching;
using Locadora.Domain.Entities;
using Locadora.Domain.Idempotencia;
using Locadora.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Locadora.Application.Services;

public class AmigoService : IAmigoService
{
    private const int TamanhoPaginaMaximo = 100;

    private readonly IAmigoRepository _amigos;
    private readonly IAmigoCache _cache;
    private readonly IArmazenamentoIdempotencia _idempotencia;
    private readonly ILogger<AmigoService> _logger;

    public AmigoService(IAmigoRepository amigos, IAmigoCache cache, IArmazenamentoIdempotencia idempotencia, ILogger<AmigoService> logger)
    {
        _amigos = amigos;
        _cache = cache;
        _idempotencia = idempotencia;
        _logger = logger;
    }

    public async Task<PagedResultDto<AmigoDto>> ListarAsync(string? busca, int page = 1, int pageSize = 10)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, TamanhoPaginaMaximo);

        _logger.LogDebug("Listando amigos. Busca={Busca} Page={Page} PageSize={PageSize}", busca, page, pageSize);

        var query = await _amigos.ListarAsync();

        if (!string.IsNullOrWhiteSpace(busca))
            query = query.Where(a => a.Nome.Contains(busca) || a.Sobrenome.Contains(busca));

        query = query.OrderBy(a => a.Nome);

        var totalCount = await query.CountAsync();
        var amigos = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PagedResultDto<AmigoDto>
        {
            Items = amigos.Select(a => a.ToDto()).ToList(),
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<AmigoDto?> ObterPorIdAsync(int id)
    {
        var cacheado = await _cache.ObterAsync(id);
        if (cacheado is not null)
        {
            _logger.LogDebug("Amigo {AmigoId} obtido do cache.", id);
            return cacheado.ToDto();
        }

        var amigo = await _amigos.ObterPorIdAsync(id);
        if (amigo is null)
        {
            _logger.LogWarning("Amigo {AmigoId} não encontrado.", id);
            return null;
        }

        await _cache.DefinirAsync(amigo);
        return amigo.ToDto();
    }

    public async Task<AmigoDto> CriarAsync(AmigoInputDto input, string? chaveIdempotencia)
    {
        var existente = await ExecutorIdempotente.ObterAsync<AmigoDto>(_idempotencia, "amigo:criar", chaveIdempotencia);
        if (existente is not null)
        {
            _logger.LogInformation("Criação de amigo idempotente: retornando resultado já existente para a chave {ChaveIdempotencia}.", chaveIdempotencia);
            return existente;
        }

        var agora = DateTime.UtcNow;
        var amigo = new Amigo
        {
            Nome = input.Nome,
            Sobrenome = input.Sobrenome,
            Idade = input.Idade,
            DataCadastro = agora,
            DataAtualizacao = agora
        };

        await _amigos.AdicionarAsync(amigo);
        await _cache.DefinirAsync(amigo);

        _logger.LogInformation("Amigo {AmigoId} criado: {Nome} {Sobrenome}.", amigo.Id, amigo.Nome, amigo.Sobrenome);

        var dto = amigo.ToDto();
        await ExecutorIdempotente.SalvarAsync(_idempotencia, "amigo:criar", chaveIdempotencia, dto);
        return dto;
    }

    public async Task<bool> AtualizarAsync(int id, AmigoInputDto input)
    {
        var amigo = await _amigos.ObterPorIdAsync(id);
        if (amigo is null)
        {
            _logger.LogWarning("Atualização falhou: amigo {AmigoId} não encontrado.", id);
            return false;
        }

        amigo.Nome = input.Nome;
        amigo.Sobrenome = input.Sobrenome;
        amigo.Idade = input.Idade;
        amigo.DataAtualizacao = DateTime.UtcNow;

        await _amigos.AtualizarAsync(amigo);
        await _cache.DefinirAsync(amigo);

        _logger.LogInformation("Amigo {AmigoId} atualizado.", id);
        return true;
    }

    public async Task<bool> RemoverAsync(int id)
    {
        var amigo = await _amigos.ObterPorIdAsync(id);
        if (amigo is null)
        {
            _logger.LogWarning("Remoção falhou: amigo {AmigoId} não encontrado.", id);
            return false;
        }

        await _amigos.RemoverAsync(amigo);
        await _cache.RemoverAsync(id);

        _logger.LogInformation("Amigo {AmigoId} removido.", id);
        return true;
    }
}
