using Locadora.Domain.Common;
using Locadora.Domain.Enums;

namespace Locadora.Domain.Entities;

public class Jogo : Entity
{
    public string Nome { get; set; } = string.Empty;
    public string? ImagemCapa { get; set; }
    public ConsoleTipo Console { get; set; }
    public DateTime DataCadastro { get; set; }
    public DateTime DataAtualizacao { get; set; }
}
