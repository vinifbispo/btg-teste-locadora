using Locadora.Application.Dtos;
using Locadora.Application.Idempotencia;
using Locadora.Application.Interfaces;
using Locadora.Application.Mappings;
using Locadora.Domain.Caching;
using Locadora.Domain.Entities;
using Locadora.Domain.Enums;
using Locadora.Domain.Idempotencia;
using Locadora.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Locadora.Application.Services;

public class JogoService : IJogoService
{
    private readonly IJogoRepository _jogos;
    private readonly IJogoCache _cache;
    private readonly IArmazenamentoIdempotencia _idempotencia;

    public JogoService(IJogoRepository jogos, IJogoCache cache, IArmazenamentoIdempotencia idempotencia)
    {
        _jogos = jogos;
        _cache = cache;
        _idempotencia = idempotencia;
    }

    public async Task<IEnumerable<JogoDto>> ListarAsync(ConsoleTipo? console, string? busca)
    {
        var query = await _jogos.ListarAsync();

        if (console is not null)
            query = query.Where(j => j.Console == console);

        if (!string.IsNullOrWhiteSpace(busca))
            query = query.Where(j => j.Nome.Contains(busca));

        var jogos = await query.OrderBy(j => j.Nome).ToListAsync();
        return jogos.Select(j => j.ToDto()).ToList();
    }

    public async Task<JogoDto?> ObterPorIdAsync(int id)
    {
        var cacheado = await _cache.ObterAsync(id);
        if (cacheado is not null)
            return cacheado.ToDto();

        var jogo = await _jogos.ObterPorIdAsync(id);
        if (jogo is null)
            return null;

        await _cache.DefinirAsync(jogo);
        return jogo.ToDto();
    }

    public async Task<JogoDto> CriarAsync(JogoInputDto input, string? chaveIdempotencia)
    {
        var existente = await ExecutorIdempotente.ObterAsync<JogoDto>(_idempotencia, "jogo:criar", chaveIdempotencia);
        if (existente is not null)
            return existente;

        var agora = DateTime.UtcNow;
        var jogo = new Jogo
        {
            Nome = input.Nome,
            ImagemCapa = input.ImagemCapa,
            Console = input.Console,
            DataCadastro = agora,
            DataAtualizacao = agora
        };

        await _jogos.AdicionarAsync(jogo);
        await _cache.DefinirAsync(jogo);

        var dto = jogo.ToDto();
        await ExecutorIdempotente.SalvarAsync(_idempotencia, "jogo:criar", chaveIdempotencia, dto);
        return dto;
    }

    public async Task<bool> AtualizarAsync(int id, JogoInputDto input)
    {
        var jogo = await _jogos.ObterPorIdAsync(id);
        if (jogo is null)
            return false;

        jogo.Nome = input.Nome;
        jogo.ImagemCapa = input.ImagemCapa;
        jogo.Console = input.Console;
        jogo.DataAtualizacao = DateTime.UtcNow;

        await _jogos.AtualizarAsync(jogo);
        await _cache.DefinirAsync(jogo);
        return true;
    }

    public async Task<bool> RemoverAsync(int id)
    {
        var jogo = await _jogos.ObterPorIdAsync(id);
        if (jogo is null)
            return false;

        await _jogos.RemoverAsync(jogo);
        await _cache.RemoverAsync(id);
        return true;
    }
}
