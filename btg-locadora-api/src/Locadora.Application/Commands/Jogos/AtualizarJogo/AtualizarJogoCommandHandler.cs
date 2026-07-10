using Locadora.Domain.Caching;
using Locadora.Domain.Entities;
using Locadora.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Locadora.Application.Commands.Jogos.AtualizarJogo;

public class AtualizarJogoCommandHandler : IRequestHandler<AtualizarJogoCommand, bool>
{
    private readonly IJogoRepository _jogos;
    private readonly IGeneroRepository _generos;
    private readonly IDesenvolvedorRepository _desenvolvedores;
    private readonly IPublicadoraRepository _publicadoras;
    private readonly IJogoCache _cache;
    private readonly ILogger<AtualizarJogoCommandHandler> _logger;

    public AtualizarJogoCommandHandler(
        IJogoRepository jogos,
        IGeneroRepository generos,
        IDesenvolvedorRepository desenvolvedores,
        IPublicadoraRepository publicadoras,
        IJogoCache cache,
        ILogger<AtualizarJogoCommandHandler> logger)
    {
        _jogos = jogos;
        _generos = generos;
        _desenvolvedores = desenvolvedores;
        _publicadoras = publicadoras;
        _cache = cache;
        _logger = logger;
    }

    public async Task<bool> Handle(AtualizarJogoCommand request, CancellationToken cancellationToken)
    {
        var jogo = await _jogos.ObterPorIdAsync(request.Id);
        if (jogo is null)
        {
            _logger.LogWarning("Atualização falhou: jogo {JogoId} não encontrado.", request.Id);
            return false;
        }

        var input = request.Input;
        jogo.Nome = input.Nome;
        jogo.Generos = await JogoAssociacoesResolver.ResolverGenerosAsync(_generos, input.GeneroIds);
        jogo.Desenvolvedores = await JogoAssociacoesResolver.ResolverDesenvolvedoresAsync(_desenvolvedores, input.DesenvolvedorIds);
        jogo.Publicadoras = await JogoAssociacoesResolver.ResolverPublicadorasAsync(_publicadoras, input.PublicadoraIds);

        jogo.DatasLancamento.Clear();
        jogo.DatasLancamento.AddRange(input.DatasLancamento.Select(d => new DataLancamento { Regiao = d.Regiao, Data = d.Data }));

        await _jogos.AtualizarAsync(jogo);
        await _cache.DefinirAsync(jogo);

        _logger.LogInformation("Jogo {JogoId} atualizado.", request.Id);
        return true;
    }
}
