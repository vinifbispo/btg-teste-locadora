using Locadora.Domain.Common;

namespace Locadora.Domain.Entities;

public class Emprestimo : Entity
{
    public int JogoId { get; set; }
    public Jogo Jogo { get; set; } = null!;

    public int AmigoId { get; set; }
    public Amigo Amigo { get; set; } = null!;

    public DateTime DataEmprestimo { get; set; }
    public DateTime? DataDevolucao { get; set; }
}
