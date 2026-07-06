using Locadora.Application.Dtos;
using Locadora.Domain.Entities;

namespace Locadora.Application.Mappings;

internal static class EmprestimoMappings
{
    public static EmprestimoDto ToDto(this Emprestimo emprestimo) => new()
    {
        Id = emprestimo.Id,
        JogoId = emprestimo.JogoId,
        JogoNome = emprestimo.Jogo?.Nome ?? string.Empty,
        AmigoId = emprestimo.AmigoId,
        AmigoNome = emprestimo.Amigo is not null ? $"{emprestimo.Amigo.Nome} {emprestimo.Amigo.Sobrenome}" : string.Empty,
        DataEmprestimo = emprestimo.DataEmprestimo,
        DataDevolucao = emprestimo.DataDevolucao
    };
}
