using Locadora.Domain.Enums;

namespace Locadora.Application.Dtos;

public class JogoInputDto
{
    public string Nome { get; set; } = string.Empty;
    public string? ImagemCapa { get; set; }
    public ConsoleTipo Console { get; set; }
}

public class JogoDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? ImagemCapa { get; set; }
    public ConsoleTipo Console { get; set; }
    public DateTime DataCadastro { get; set; }
    public DateTime DataAtualizacao { get; set; }
}
