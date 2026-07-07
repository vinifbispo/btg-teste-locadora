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
            .Select(g => g.ToDto())
            .ToList(),
        Desenvolvedores = jogo.Desenvolvedores
            .Select(d => d.ToDto())
            .ToList(),
        Publicadoras = jogo.Publicadoras
            .Select(p => p.ToDto())
            .ToList(),
        DatasLancamento = jogo.DatasLancamento
            .Select(d => new DataLancamentoDto { Regiao = d.Regiao, Data = d.Data })
            .ToList()
    };

    public static GeneroDto ToDto(this Genero genero) => new() { Id = genero.Id, Nome = genero.Nome };

    public static DesenvolvedorDto ToDto(this Desenvolvedor desenvolvedor) => new() { Id = desenvolvedor.Id, Nome = desenvolvedor.Nome };

    public static PublicadoraDto ToDto(this Publicadora publicadora) => new() { Id = publicadora.Id, Nome = publicadora.Nome };
}
