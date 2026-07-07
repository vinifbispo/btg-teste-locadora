using Locadora.Web.Application.Interfaces;
using Locadora.Web.Domain.Exceptions;
using Locadora.Web.Domain.Models;
using Locadora.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Locadora.Web.Controllers;

public class EmprestimosController : Controller
{
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

    public async Task<IActionResult> Index(int? jogoId, int? amigoId, bool? apenasAtivos)
    {
        var emprestimos = await _emprestimos.ListarAsync(jogoId, amigoId, apenasAtivos);

        var model = new EmprestimoFiltroViewModel
        {
            JogoId = jogoId,
            AmigoId = amigoId,
            ApenasAtivos = apenasAtivos,
            Emprestimos = emprestimos,
            Jogos = await ObterOpcoesJogosAsync(),
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
            Jogos = await ObterOpcoesJogosAsync(),
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
            model.Jogos = await ObterOpcoesJogosAsync();
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
            model.Jogos = await ObterOpcoesJogosAsync();
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

    private async Task<IEnumerable<SelectListItem>> ObterOpcoesJogosAsync()
    {
        var jogos = await _jogos.ListarAsync();
        return jogos
            .OrderBy(j => j.Nome)
            .Select(j => new SelectListItem(j.Nome, j.Id.ToString()));
    }

    private async Task<IEnumerable<SelectListItem>> ObterOpcoesAmigosAsync()
    {
        var amigos = await _amigos.ListarAsync();
        return amigos
            .OrderBy(a => a.Nome)
            .Select(a => new SelectListItem($"{a.Nome} {a.Sobrenome}", a.Id.ToString()));
    }
}
