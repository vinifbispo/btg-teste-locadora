namespace Locadora.Web.Domain.Models;

public class JogoInput
{
    public string Nome { get; set; } = string.Empty;
    public List<int> GeneroIds { get; set; } = new();
    public List<int> DesenvolvedorIds { get; set; } = new();
    public List<int> PublicadoraIds { get; set; } = new();
    public List<DataLancamento> DatasLancamento { get; set; } = new();
}
