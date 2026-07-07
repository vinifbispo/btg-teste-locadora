using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Locadora.Web.Models;

public class EmprestimoFormViewModel
{
    [Range(1, int.MaxValue, ErrorMessage = "Selecione o jogo.")]
    [Display(Name = "Jogo")]
    public int JogoId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Selecione o amigo.")]
    [Display(Name = "Amigo")]
    public int AmigoId { get; set; }

    public string ChaveIdempotencia { get; set; } = Guid.NewGuid().ToString();

    public IEnumerable<SelectListItem> Jogos { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> Amigos { get; set; } = new List<SelectListItem>();
}
