namespace Locadora.Application.Dtos;

public class DataLancamentoDto
{
    public string Regiao { get; set; } = string.Empty;
    public string Data { get; set; } = string.Empty;
}

public class GeneroDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
}

public class DesenvolvedorDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
}

public class PublicadoraDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
}

public class JogoInputDto
{
    public string Nome { get; set; } = string.Empty;
    public List<int> GeneroIds { get; set; } = new();
    public List<int> DesenvolvedorIds { get; set; } = new();
    public List<int> PublicadoraIds { get; set; } = new();
    public List<DataLancamentoDto> DatasLancamento { get; set; } = new();
}

public class JogoDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public List<GeneroDto> Generos { get; set; } = new();
    public List<DesenvolvedorDto> Desenvolvedores { get; set; } = new();
    public List<PublicadoraDto> Publicadoras { get; set; } = new();
    public List<DataLancamentoDto> DatasLancamento { get; set; } = new();
}
