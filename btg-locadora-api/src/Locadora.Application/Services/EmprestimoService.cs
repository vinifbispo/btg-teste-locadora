using Locadora.Application.Dtos;
using Locadora.Application.Idempotencia;
using Locadora.Application.Interfaces;
using Locadora.Application.Mappings;
using Locadora.Domain.Caching;
using Locadora.Domain.Entities;
using Locadora.Domain.Exceptions;
using Locadora.Domain.Idempotencia;
using Locadora.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Locadora.Application.Services;

public class EmprestimoService : IEmprestimoService
{
    private const int TamanhoPaginaMaximo = 100;

    private readonly IEmprestimoRepository _emprestimos;
    private readonly IJogoRepository _jogos;
    private readonly IAmigoRepository _amigos;
    private readonly IEmprestimoCache _cache;
    private readonly IArmazenamentoIdempotencia _idempotencia;
    private readonly ILogger<EmprestimoService> _logger;

    public EmprestimoService(
        IEmprestimoRepository emprestimos,
        IJogoRepository jogos,
        IAmigoRepository amigos,
        IEmprestimoCache cache,
        IArmazenamentoIdempotencia idempotencia,
        ILogger<EmprestimoService> logger)
    {
        _emprestimos = emprestimos;
        _jogos = jogos;
        _amigos = amigos;
        _cache = cache;
        _idempotencia = idempotencia;
        _logger = logger;
    }

    public async Task<PagedResultDto<EmprestimoDto>> ListarAsync(int? jogoId, int? amigoId, bool? apenasAtivos, int page = 1, int pageSize = 10)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, TamanhoPaginaMaximo);

        _logger.LogDebug(
            "Listando empréstimos. JogoId={JogoId} AmigoId={AmigoId} ApenasAtivos={ApenasAtivos} Page={Page} PageSize={PageSize}",
            jogoId, amigoId, apenasAtivos, page, pageSize);

        var query = await _emprestimos.ListarAsync();

        if (jogoId is not null)
            query = query.Where(e => e.JogoId == jogoId);

        if (amigoId is not null)
            query = query.Where(e => e.AmigoId == amigoId);

        if (apenasAtivos is true)
            query = query.Where(e => e.DataDevolucao == null);

        query = query.OrderByDescending(e => e.DataEmprestimo);

        var totalCount = await query.CountAsync();
        var emprestimos = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PagedResultDto<EmprestimoDto>
        {
            Items = emprestimos.Select(e => e.ToDto()).ToList(),
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<EmprestimoDto?> ObterPorIdAsync(int id)
    {
        var cacheado = await _cache.ObterAsync(id);
        if (cacheado is not null)
        {
            _logger.LogDebug("Empréstimo {EmprestimoId} obtido do cache.", id);
            return cacheado.ToDto();
        }

        var emprestimo = await _emprestimos.ObterPorIdAsync(id);
        if (emprestimo is null)
        {
            _logger.LogWarning("Empréstimo {EmprestimoId} não encontrado.", id);
            return null;
        }

        await _cache.DefinirAsync(emprestimo);
        return emprestimo.ToDto();
    }

    public async Task<EmprestimoDto> EmprestarAsync(EmprestimoInputDto input, string? chaveIdempotencia)
    {
        var existente = await ExecutorIdempotente.ObterAsync<EmprestimoDto>(_idempotencia, "emprestimo:criar", chaveIdempotencia);
        if (existente is not null)
        {
            _logger.LogInformation("Criação de empréstimo idempotente: retornando resultado já existente para a chave {ChaveIdempotencia}.", chaveIdempotencia);
            return existente;
        }

        var jogo = await _jogos.ObterPorIdAsync(input.JogoId);
        if (jogo is null)
        {
            _logger.LogWarning("Empréstimo falhou: jogo {JogoId} não encontrado.", input.JogoId);
            throw new NotFoundException("Jogo não encontrado.");
        }

        var amigo = await _amigos.ObterPorIdAsync(input.AmigoId);
        if (amigo is null)
        {
            _logger.LogWarning("Empréstimo falhou: amigo {AmigoId} não encontrado.", input.AmigoId);
            throw new NotFoundException("Amigo não encontrado.");
        }

        var ativo = await _emprestimos.ObterAtivoPorJogoAsync(input.JogoId);
        if (ativo is not null)
        {
            _logger.LogWarning("Empréstimo falhou: jogo {JogoId} já está emprestado (empréstimo {EmprestimoId}).", input.JogoId, ativo.Id);
            throw new ConflitoException("Este jogo já está emprestado.");
        }

        var emprestimo = new Emprestimo
        {
            JogoId = jogo.Id,
            AmigoId = amigo.Id,
            DataEmprestimo = DateTime.UtcNow
        };

        await _emprestimos.AdicionarAsync(emprestimo);

        emprestimo.Jogo = jogo;
        emprestimo.Amigo = amigo;
        await _cache.DefinirAsync(emprestimo);

        _logger.LogInformation("Empréstimo {EmprestimoId} criado: jogo {JogoId} para amigo {AmigoId}.", emprestimo.Id, jogo.Id, amigo.Id);

        var dto = emprestimo.ToDto();
        await ExecutorIdempotente.SalvarAsync(_idempotencia, "emprestimo:criar", chaveIdempotencia, dto);
        return dto;
    }

    public async Task<bool> DevolverAsync(int emprestimoId)
    {
        var emprestimo = await _emprestimos.ObterPorIdAsync(emprestimoId);
        if (emprestimo is null || emprestimo.DataDevolucao is not null)
        {
            _logger.LogWarning("Devolução falhou: empréstimo {EmprestimoId} não encontrado ou já devolvido.", emprestimoId);
            return false;
        }

        emprestimo.DataDevolucao = DateTime.UtcNow;
        await _emprestimos.AtualizarAsync(emprestimo);
        await _cache.DefinirAsync(emprestimo);

        _logger.LogInformation("Empréstimo {EmprestimoId} devolvido.", emprestimoId);
        return true;
    }
}
