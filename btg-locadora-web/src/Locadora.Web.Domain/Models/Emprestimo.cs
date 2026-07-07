namespace Locadora.Web.Domain.Models;

public class Emprestimo
{
    public int Id { get; set; }
    public int JogoId { get; set; }
    public string JogoNome { get; set; } = string.Empty;
    public int AmigoId { get; set; }
    public string AmigoNome { get; set; } = string.Empty;
    public DateTime DataEmprestimo { get; set; }
    public DateTime? DataDevolucao { get; set; }
}
