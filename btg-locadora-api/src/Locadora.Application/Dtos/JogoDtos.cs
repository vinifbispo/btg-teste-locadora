namespace Locadora.Application.Dtos;

public class DataLancamentoDto
{
    public string Regiao { get; set; } = string.Empty;
    public string Data { get; set; } = string.Empty;
}

public class JogoInputDto
{
    public string Nome { get; set; } = string.Empty;
    public string? UrlImagemCapa { get; set; }
    public List<string> Generos { get; set; } = new();
    public List<string> Desenvolvedores { get; set; } = new();
    public List<string> Publicadoras { get; set; } = new();
    public List<DataLancamentoDto> DatasLancamento { get; set; } = new();
}

public class JogoDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? UrlImagemCapa { get; set; }
    public List<string> Generos { get; set; } = new();
    public List<string> Desenvolvedores { get; set; } = new();
    public List<string> Publicadoras { get; set; } = new();
    public List<DataLancamentoDto> DatasLancamento { get; set; } = new();
}
