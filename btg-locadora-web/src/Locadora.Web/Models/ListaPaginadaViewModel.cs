namespace Locadora.Web.Models;

public class ListaPaginadaViewModel<T>
{
    public IEnumerable<T> Items { get; set; } = new List<T>();
    public string? Busca { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
}
