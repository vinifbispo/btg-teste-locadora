using Locadora.Application.Dtos;
using Locadora.Domain.Entities;

namespace Locadora.Application.Mappings;

internal static class AmigoMappings
{
    public static AmigoDto ToDto(this Amigo amigo) => new()
    {
        Id = amigo.Id,
        Nome = amigo.Nome,
        Sobrenome = amigo.Sobrenome,
        Idade = amigo.Idade,
        DataCadastro = amigo.DataCadastro,
        DataAtualizacao = amigo.DataAtualizacao
    };
}
