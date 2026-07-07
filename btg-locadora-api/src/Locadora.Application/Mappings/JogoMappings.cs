using Locadora.Application.Dtos;
using Locadora.Domain.Entities;

namespace Locadora.Application.Mappings;

internal static class JogoMappings
{
    public static JogoDto ToDto(this Jogo jogo) => new()
    {
        Id = jogo.Id,
        Nome = jogo.Nome,
        Generos = jogo.Generos
            .Select(g => new GeneroDto { Id = g.Id, Nome = g.Nome })
            .ToList(),
        Desenvolvedores = jogo.Desenvolvedores
            .Select(d => new DesenvolvedorDto { Id = d.Id, Nome = d.Nome })
            .ToList(),
        Publicadoras = jogo.Publicadoras
            .Select(p => new PublicadoraDto { Id = p.Id, Nome = p.Nome })
            .ToList(),
        DatasLancamento = jogo.DatasLancamento
            .Select(d => new DataLancamentoDto { Regiao = d.Regiao, Data = d.Data })
            .ToList()
    };
}
