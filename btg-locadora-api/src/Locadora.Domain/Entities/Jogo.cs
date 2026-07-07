using Locadora.Domain.Common;

namespace Locadora.Domain.Entities;

public class Jogo : Entity
{
    public string Nome { get; set; } = string.Empty;
    public List<string> Generos { get; set; } = new();
    public List<string> Desenvolvedores { get; set; } = new();
    public List<string> Publicadoras { get; set; } = new();
    public List<DataLancamento> DatasLancamento { get; set; } = new();
}
