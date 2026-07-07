namespace Locadora.Domain.Integracoes.Dtos;

public class JogoExterno
{
    public string Nome { get; set; } = string.Empty;
    public List<string> Generos { get; set; } = new();
    public List<string> Desenvolvedores { get; set; } = new();
    public List<string> Publicadoras { get; set; } = new();
    public Dictionary<string, string> DatasLancamento { get; set; } = new();
}
