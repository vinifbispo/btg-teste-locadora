namespace Locadora.Infrastructure.Http.Dtos;

public class JogoExternoResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<string> Genre { get; set; } = new();
    public List<string> Developers { get; set; } = new();
    public List<string> Publishers { get; set; } = new();
    public Dictionary<string, string> ReleaseDates { get; set; } = new();
}
