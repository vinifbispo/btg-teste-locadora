using System.ComponentModel.DataAnnotations;
using Locadora.Web.Domain.Models;

namespace Locadora.Web.Models;

public class AmigoFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
    [Display(Name = "Nome")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "O sobrenome é obrigatório.")]
    [StringLength(100, ErrorMessage = "O sobrenome deve ter no máximo 100 caracteres.")]
    [Display(Name = "Sobrenome")]
    public string Sobrenome { get; set; } = string.Empty;

    [Range(1, 120, ErrorMessage = "A idade deve estar entre 1 e 120.")]
    [Display(Name = "Idade")]
    public int Idade { get; set; }

    public string ChaveIdempotencia { get; set; } = Guid.NewGuid().ToString();

    public AmigoInput ToInput() => new()
    {
        Nome = Nome,
        Sobrenome = Sobrenome,
        Idade = Idade
    };

    public static AmigoFormViewModel FromAmigo(Amigo amigo) => new()
    {
        Id = amigo.Id,
        Nome = amigo.Nome,
        Sobrenome = amigo.Sobrenome,
        Idade = amigo.Idade
    };
}
