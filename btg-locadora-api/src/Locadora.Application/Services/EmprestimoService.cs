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

namespace Locadora.Application.Services;

public class EmprestimoService : IEmprestimoService
{
    private readonly IEmprestimoRepository _emprestimos;
    private readonly IJogoRepository _jogos;
    private readonly IAmigoRepository _amigos;
    private readonly IEmprestimoCache _cache;
    private readonly IArmazenamentoIdempotencia _idempotencia;

    public EmprestimoService(
        IEmprestimoRepository emprestimos,
        IJogoRepository jogos,
        IAmigoRepository amigos,
        IEmprestimoCache cache,
        IArmazenamentoIdempotencia idempotencia)
    {
        _emprestimos = emprestimos;
        _jogos = jogos;
        _amigos = amigos;
        _cache = cache;
        _idempotencia = idempotencia;
    }

    public async Task<IEnumerable<EmprestimoDto>> ListarAsync(int? jogoId, int? amigoId, bool? apenasAtivos)
    {
        var query = await _emprestimos.ListarAsync();

        if (jogoId is not null)
            query = query.Where(e => e.JogoId == jogoId);

        if (amigoId is not null)
            query = query.Where(e => e.AmigoId == amigoId);

        if (apenasAtivos is true)
            query = query.Where(e => e.DataDevolucao == null);

        var emprestimos = await query.OrderByDescending(e => e.DataEmprestimo).ToListAsync();
        return emprestimos.Select(e => e.ToDto()).ToList();
    }

    public async Task<EmprestimoDto?> ObterPorIdAsync(int id)
    {
        var cacheado = await _cache.ObterAsync(id);
        if (cacheado is not null)
            return cacheado.ToDto();

        var emprestimo = await _emprestimos.ObterPorIdAsync(id);
        if (emprestimo is null)
            return null;

        await _cache.DefinirAsync(emprestimo);
        return emprestimo.ToDto();
    }

    public async Task<EmprestimoDto> EmprestarAsync(EmprestimoInputDto input, string? chaveIdempotencia)
    {
        var existente = await ExecutorIdempotente.ObterAsync<EmprestimoDto>(_idempotencia, "emprestimo:criar", chaveIdempotencia);
        if (existente is not null)
            return existente;

        var jogo = await _jogos.ObterPorIdAsync(input.JogoId)
            ?? throw new NotFoundException("Jogo não encontrado.");

        var amigo = await _amigos.ObterPorIdAsync(input.AmigoId)
            ?? throw new NotFoundException("Amigo não encontrado.");

        var ativo = await _emprestimos.ObterAtivoPorJogoAsync(input.JogoId);
        if (ativo is not null)
            throw new ConflitoException("Este jogo já está emprestado.");

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

        var dto = emprestimo.ToDto();
        await ExecutorIdempotente.SalvarAsync(_idempotencia, "emprestimo:criar", chaveIdempotencia, dto);
        return dto;
    }

    public async Task<bool> DevolverAsync(int emprestimoId)
    {
        var emprestimo = await _emprestimos.ObterPorIdAsync(emprestimoId);
        if (emprestimo is null || emprestimo.DataDevolucao is not null)
            return false;

        emprestimo.DataDevolucao = DateTime.UtcNow;
        await _emprestimos.AtualizarAsync(emprestimo);
        await _cache.DefinirAsync(emprestimo);
        return true;
    }
}
