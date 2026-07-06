using Locadora.Application.Dtos;
using Locadora.Domain.Entities;

namespace Locadora.Application.Mappings;

internal static class JogoMappings
{
    public static JogoDto ToDto(this Jogo jogo) => new()
    {
        Id = jogo.Id,
        Nome = jogo.Nome,
        ImagemCapa = jogo.ImagemCapa,
        Console = jogo.Console,
        DataCadastro = jogo.DataCadastro,
        DataAtualizacao = jogo.DataAtualizacao
    };
}
