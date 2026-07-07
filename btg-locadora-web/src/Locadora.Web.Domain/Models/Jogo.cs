namespace Locadora.Web.Domain.Models;

public class Jogo
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public List<Genero> Generos { get; set; } = new();
    public List<Desenvolvedor> Desenvolvedores { get; set; } = new();
    public List<Publicadora> Publicadoras { get; set; } = new();
    public List<DataLancamento> DatasLancamento { get; set; } = new();
}
