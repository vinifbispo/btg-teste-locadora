using Locadora.Domain.Common;

namespace Locadora.Domain.Entities;

public class Amigo : Entity
{
    public string Nome { get; set; } = string.Empty;
    public string Sobrenome { get; set; } = string.Empty;
    public int Idade { get; set; }
    public DateTime DataCadastro { get; set; }
    public DateTime DataAtualizacao { get; set; }
}
