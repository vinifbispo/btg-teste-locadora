using Locadora.Web.Application.Interfaces;
using Locadora.Web.Domain.Exceptions;
using Locadora.Web.Domain.Models;
using Locadora.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Locadora.Web.Controllers;

public class EmprestimosController : Controller
{
    private const int TamanhoMinimoBusca = 3;

    private readonly IEmprestimoService _emprestimos;
    private readonly IJogoService _jogos;
    private readonly IAmigoService _amigos;
    private readonly ILogger<EmprestimosController> _logger;

    public EmprestimosController(
        IEmprestimoService emprestimos,
        IJogoService jogos,
        IAmigoService amigos,
        ILogger<EmprestimosController> logger)
    {
        _emprestimos = emprestimos;
        _jogos = jogos;
        _amigos = amigos;
        _logger = logger;
    }

    public async Task<IActionResult> Index(int? jogoId, int? amigoId, bool? apenasAtivos, int page = 1)
    {
        var resultado = await _emprestimos.ListarAsync(jogoId, amigoId, apenasAtivos, page);

        string? jogoNome = null;
        if (jogoId is > 0)
        {
            var jogo = await _jogos.ObterPorIdAsync(jogoId.Value);
            jogoNome = jogo?.Nome;
        }

        var model = new EmprestimoFiltroViewModel
        {
            JogoId = jogoId,
            JogoNome = jogoNome,
            AmigoId = amigoId,
            ApenasAtivos = apenasAtivos,
            Emprestimos = resultado.Items,
            Page = resultado.Page,
            PageSize = resultado.PageSize,
            TotalCount = resultado.TotalCount,
            TotalPages = resultado.TotalPages,
            Amigos = await ObterOpcoesAmigosAsync()
        };

        return View(model);
    }

    public async Task<IActionResult> Details(int id)
    {
        var emprestimo = await _emprestimos.ObterPorIdAsync(id);
        if (emprestimo is null)
            return NotFound();

        return View(emprestimo);
    }

    public async Task<IActionResult> Create()
    {
        var model = new EmprestimoFormViewModel
        {
            Amigos = await ObterOpcoesAmigosAsync()
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EmprestimoFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.Amigos = await ObterOpcoesAmigosAsync();
            return View(model);
        }

        try
        {
            var input = new EmprestimoInput { JogoId = model.JogoId, AmigoId = model.AmigoId };
            var emprestimo = await _emprestimos.CriarAsync(input, model.ChaveIdempotencia);
            TempData["Sucesso"] = $"Empréstimo do jogo \"{emprestimo.JogoNome}\" para \"{emprestimo.AmigoNome}\" registrado com sucesso.";
            return RedirectToAction(nameof(Index));
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "Falha ao criar empréstimo.");
            ModelState.AddModelError(string.Empty, ex.Message);
            model.Amigos = await ObterOpcoesAmigosAsync();
            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Devolver(int id)
    {
        try
        {
            var sucesso = await _emprestimos.DevolverAsync(id);
            if (!sucesso)
                return NotFound();

            TempData["Sucesso"] = "Devolução registrada com sucesso.";
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "Falha ao devolver empréstimo {EmprestimoId}.", id);
            TempData["Erro"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> BuscarJogos(string texto)
    {
        if (string.IsNullOrWhiteSpace(texto) || texto.Trim().Length < TamanhoMinimoBusca)
            return Json(Array.Empty<object>());

        try
        {
            var resultado = await _jogos.ListarAsync(texto.Trim(), page: 1, pageSize: 20);
            return Json(resultado.Items.Select(j => new { id = j.Id, nome = j.Nome }));
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "Falha ao buscar jogos para seleção de empréstimo.");
            return Json(Array.Empty<object>());
        }
    }

    private async Task<IEnumerable<SelectListItem>> ObterOpcoesAmigosAsync()
    {
        var resultado = await _amigos.ListarAsync(busca: null, page: 1, pageSize: 100);
        return resultado.Items
            .OrderBy(a => a.Nome)
            .Select(a => new SelectListItem($"{a.Nome} {a.Sobrenome}", a.Id.ToString()));
    }
}
