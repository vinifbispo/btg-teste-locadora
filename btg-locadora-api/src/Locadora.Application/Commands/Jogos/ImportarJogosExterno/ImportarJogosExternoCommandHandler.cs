using Locadora.Application.Mappings;
using Locadora.Domain.Caching;
using Locadora.Domain.Entities;
using Locadora.Domain.Integracoes;
using Locadora.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Locadora.Application.Commands.Jogos.ImportarJogosExterno;

public class ImportarJogosExternoCommandHandler : IRequestHandler<ImportarJogosExternoCommand>
{
    private readonly IJogoRepository _jogos;
    private readonly IGeneroRepository _generos;
    private readonly IDesenvolvedorRepository _desenvolvedores;
    private readonly IPublicadoraRepository _publicadoras;
    private readonly IJogoCache _cache;
    private readonly IJogoExternoApiClient _jogoExterno;
    private readonly ILogger<ImportarJogosExternoCommandHandler> _logger;

    public ImportarJogosExternoCommandHandler(
        IJogoRepository jogos,
        IGeneroRepository generos,
        IDesenvolvedorRepository desenvolvedores,
        IPublicadoraRepository publicadoras,
        IJogoCache cache,
        IJogoExternoApiClient jogoExterno,
        ILogger<ImportarJogosExternoCommandHandler> logger)
    {
        _jogos = jogos;
        _generos = generos;
        _desenvolvedores = desenvolvedores;
        _publicadoras = publicadoras;
        _cache = cache;
        _jogoExterno = jogoExterno;
        _logger = logger;
    }

    public async Task Handle(ImportarJogosExternoCommand request, CancellationToken cancellationToken)
    {
        var query = await _jogos.ListarAsync();
        if (await query.AnyAsync(cancellationToken))
        {
            _logger.LogInformation("Importação de jogos externos ignorada: já existem jogos cadastrados.");
            return;
        }

        _logger.LogInformation("Importação de jogos externos iniciada.");

        var externos = await _jogoExterno.ListarAsync(cancellationToken);

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
}
