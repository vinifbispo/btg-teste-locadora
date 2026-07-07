using System.ComponentModel.DataAnnotations;
using Locadora.Web.Domain.Models;

namespace Locadora.Web.Models;

public class OpcaoSelecionada
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
}

public class DataLancamentoFormItem
{
    [Display(Name = "Região")]
    public string Regiao { get; set; } = string.Empty;

    [Display(Name = "Data")]
    public string Data { get; set; } = string.Empty;
}

public class JogoFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(150, ErrorMessage = "O nome deve ter no máximo 150 caracteres.")]
    [Display(Name = "Nome")]
    public string Nome { get; set; } = string.Empty;

    [Display(Name = "Gêneros")]
    public List<OpcaoSelecionada> Generos { get; set; } = new();

    [Display(Name = "Desenvolvedores")]
    public List<OpcaoSelecionada> Desenvolvedores { get; set; } = new();

    [Display(Name = "Publicadoras")]
    public List<OpcaoSelecionada> Publicadoras { get; set; } = new();

    [Display(Name = "Datas de lançamento")]
    public List<DataLancamentoFormItem> DatasLancamento { get; set; } = new();

    public string ChaveIdempotencia { get; set; } = Guid.NewGuid().ToString();

    public JogoInput ToInput() => new()
    {
        Nome = Nome,
        GeneroIds = Generos.Select(g => g.Id).ToList(),
        DesenvolvedorIds = Desenvolvedores.Select(d => d.Id).ToList(),
        PublicadoraIds = Publicadoras.Select(p => p.Id).ToList(),
        DatasLancamento = DatasLancamento
            .Where(d => !string.IsNullOrWhiteSpace(d.Regiao) || !string.IsNullOrWhiteSpace(d.Data))
            .Select(d => new DataLancamento { Regiao = d.Regiao, Data = d.Data })
            .ToList()
    };

    public static JogoFormViewModel FromJogo(Jogo jogo) => new()
    {
        Id = jogo.Id,
        Nome = jogo.Nome,
        Generos = jogo.Generos.Select(g => new OpcaoSelecionada { Id = g.Id, Nome = g.Nome }).ToList(),
        Desenvolvedores = jogo.Desenvolvedores.Select(d => new OpcaoSelecionada { Id = d.Id, Nome = d.Nome }).ToList(),
        Publicadoras = jogo.Publicadoras.Select(p => new OpcaoSelecionada { Id = p.Id, Nome = p.Nome }).ToList(),
        DatasLancamento = jogo.DatasLancamento.Select(d => new DataLancamentoFormItem { Regiao = d.Regiao, Data = d.Data }).ToList()
    };
}
