using Locadora.Web.Domain.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Locadora.Web.Models;

public class EmprestimoFiltroViewModel
{
    public int? JogoId { get; set; }
    public string? JogoNome { get; set; }
    public int? AmigoId { get; set; }
    public bool? ApenasAtivos { get; set; }

    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }

    public IEnumerable<Emprestimo> Emprestimos { get; set; } = new List<Emprestimo>();
    public IEnumerable<SelectListItem> Amigos { get; set; } = new List<SelectListItem>();
}
