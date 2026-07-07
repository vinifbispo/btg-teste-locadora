namespace Locadora.Web.Models;

public class PaginacaoViewModel
{
    public int Page { get; set; }
    public int TotalPages { get; set; }
    public string Action { get; set; } = "Index";
    public Dictionary<string, string?> RouteValues { get; set; } = new();
}
