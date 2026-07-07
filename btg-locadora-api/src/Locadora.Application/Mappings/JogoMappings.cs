using Locadora.Application.Dtos;
using Locadora.Domain.Entities;

namespace Locadora.Application.Mappings;

internal static class JogoMappings
{
    public static JogoDto ToDto(this Jogo jogo) => new()
    {
        Id = jogo.Id,
        Nome = jogo.Nome,
        UrlImagemCapa = jogo.UrlImagemCapa,
        Generos = jogo.Generos,
        Desenvolvedores = jogo.Desenvolvedores,
        Publicadoras = jogo.Publicadoras,
        DatasLancamento = jogo.DatasLancamento
            .Select(d => new DataLancamentoDto { Regiao = d.Regiao, Data = d.Data })
            .ToList()
    };
}
