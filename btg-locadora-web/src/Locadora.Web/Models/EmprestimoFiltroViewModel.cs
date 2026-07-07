using Locadora.Web.Domain.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Locadora.Web.Models;

public class EmprestimoFiltroViewModel
{
    public int? JogoId { get; set; }
    public int? AmigoId { get; set; }
    public bool? ApenasAtivos { get; set; }

    public IEnumerable<Emprestimo> Emprestimos { get; set; } = new List<Emprestimo>();
    public IEnumerable<SelectListItem> Jogos { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> Amigos { get; set; } = new List<SelectListItem>();
}
