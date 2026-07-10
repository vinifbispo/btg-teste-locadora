using Locadora.Application.Dtos;
using Locadora.Application.Idempotencia;
using Locadora.Application.Mappings;
using Locadora.Domain.Caching;
using Locadora.Domain.Entities;
using Locadora.Domain.Idempotencia;
using Locadora.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Locadora.Application.Commands.Jogos.CriarJogo;

public class CriarJogoCommandHandler : IRequestHandler<CriarJogoCommand, JogoDto>
{
    private readonly IJogoRepository _jogos;
    private readonly IGeneroRepository _generos;
    private readonly IDesenvolvedorRepository _desenvolvedores;
    private readonly IPublicadoraRepository _publicadoras;
    private readonly IJogoCache _cache;
    private readonly IArmazenamentoIdempotencia _idempotencia;
    private readonly ILogger<CriarJogoCommandHandler> _logger;

    public CriarJogoCommandHandler(
        IJogoRepository jogos,
        IGeneroRepository generos,
        IDesenvolvedorRepository desenvolvedores,
        IPublicadoraRepository publicadoras,
        IJogoCache cache,
        IArmazenamentoIdempotencia idempotencia,
        ILogger<CriarJogoCommandHandler> logger)
    {
        _jogos = jogos;
        _generos = generos;
        _desenvolvedores = desenvolvedores;
        _publicadoras = publicadoras;
        _cache = cache;
        _idempotencia = idempotencia;
        _logger = logger;
    }

    public async Task<JogoDto> Handle(CriarJogoCommand request, CancellationToken cancellationToken)
    {
        var existente = await ExecutorIdempotente.ObterAsync<JogoDto>(_idempotencia, "jogo:criar", request.ChaveIdempotencia);
        if (existente is not null)
        {
            _logger.LogInformation("Criação de jogo idempotente: retornando resultado já existente para a chave {ChaveIdempotencia}.", request.ChaveIdempotencia);
            return existente;
        }

        var input = request.Input;
        var jogo = new Jogo
        {
            Nome = input.Nome,
            Generos = await JogoAssociacoesResolver.ResolverGenerosAsync(_generos, input.GeneroIds),
            Desenvolvedores = await JogoAssociacoesResolver.ResolverDesenvolvedoresAsync(_desenvolvedores, input.DesenvolvedorIds),
            Publicadoras = await JogoAssociacoesResolver.ResolverPublicadorasAsync(_publicadoras, input.PublicadoraIds),
            DatasLancamento = input.DatasLancamento
                .Select(d => new DataLancamento { Regiao = d.Regiao, Data = d.Data })
                .ToList()
        };

        await _jogos.AdicionarAsync(jogo);
        await _cache.DefinirAsync(jogo);

        _logger.LogInformation("Jogo {JogoId} criado: {Nome}.", jogo.Id, jogo.Nome);

        var dto = jogo.ToDto();
        await ExecutorIdempotente.SalvarAsync(_idempotencia, "jogo:criar", request.ChaveIdempotencia, dto);
        return dto;
    }
}
