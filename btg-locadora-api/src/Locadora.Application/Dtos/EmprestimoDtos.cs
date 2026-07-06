namespace Locadora.Application.Dtos;

public class EmprestimoInputDto
{
    public int JogoId { get; set; }
    public int AmigoId { get; set; }
}

public class EmprestimoDto
{
    public int Id { get; set; }
    public int JogoId { get; set; }
    public string JogoNome { get; set; } = string.Empty;
    public int AmigoId { get; set; }
    public string AmigoNome { get; set; } = string.Empty;
    public DateTime DataEmprestimo { get; set; }
    public DateTime? DataDevolucao { get; set; }
}
