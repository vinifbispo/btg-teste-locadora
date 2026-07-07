using Locadora.Domain.Common;

namespace Locadora.Domain.Entities;

public class Jogo : Entity
{
    public string Nome { get; set; } = string.Empty;
    public List<Genero> Generos { get; set; } = new();
    public List<Desenvolvedor> Desenvolvedores { get; set; } = new();
    public List<Publicadora> Publicadoras { get; set; } = new();
    public List<DataLancamento> DatasLancamento { get; set; } = new();
}
