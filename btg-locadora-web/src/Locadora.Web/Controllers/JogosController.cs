using Locadora.Web.Application.Interfaces;
using Locadora.Web.Domain.Exceptions;
using Locadora.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Locadora.Web.Controllers;

public class JogosController : Controller
{
    private const int TamanhoMinimoBusca = 3;

    private readonly IJogoService _jogos;
    private readonly IGeneroService _generos;
    private readonly IDesenvolvedorService _desenvolvedores;
    private readonly IPublicadoraService _publicadoras;
    private readonly ILogger<JogosController> _logger;

    public JogosController(
        IJogoService jogos,
        IGeneroService generos,
        IDesenvolvedorService desenvolvedores,
        IPublicadoraService publicadoras,
        ILogger<JogosController> logger)
    {
        _jogos = jogos;
        _generos = generos;
        _desenvolvedores = desenvolvedores;
        _publicadoras = publicadoras;
        _logger = logger;
    }

    public async Task<IActionResult> Index(string? busca)
    {
        var jogos = await _jogos.ListarAsync(busca);
        ViewData["Busca"] = busca;
        return View(jogos);
    }

    public async Task<IActionResult> Details(int id)
    {
        var jogo = await _jogos.ObterPorIdAsync(id);
        if (jogo is null)
            return NotFound();

        return View(jogo);
    }

    public IActionResult Create() => View(new JogoFormViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(JogoFormViewModel model)
    {
        if (model.Generos.Count == 0)
            ModelState.AddModelError(nameof(model.Generos), "Selecione ao menos um gênero.");
        if (model.Desenvolvedores.Count == 0)
            ModelState.AddModelError(nameof(model.Desenvolvedores), "Selecione ao menos um desenvolvedor.");
        if (model.Publicadoras.Count == 0)
            ModelState.AddModelError(nameof(model.Publicadoras), "Selecione ao menos uma publicadora.");

        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var jogo = await _jogos.CriarAsync(model.ToInput(), model.ChaveIdempotencia);
            TempData["Sucesso"] = $"Jogo \"{jogo.Nome}\" cadastrado com sucesso.";
            return RedirectToAction(nameof(Index));
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "Falha ao criar jogo.");
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }

    public async Task<IActionResult> Edit(int id)
    {
        var jogo = await _jogos.ObterPorIdAsync(id);
        if (jogo is null)
            return NotFound();

        return View(JogoFormViewModel.FromJogo(jogo));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, JogoFormViewModel model)
    {
        if (model.Generos.Count == 0)
            ModelState.AddModelError(nameof(model.Generos), "Selecione ao menos um gênero.");
        if (model.Desenvolvedores.Count == 0)
            ModelState.AddModelError(nameof(model.Desenvolvedores), "Selecione ao menos um desenvolvedor.");
        if (model.Publicadoras.Count == 0)
            ModelState.AddModelError(nameof(model.Publicadoras), "Selecione ao menos uma publicadora.");

        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var sucesso = await _jogos.AtualizarAsync(id, model.ToInput());
            if (!sucesso)
                return NotFound();

            TempData["Sucesso"] = "Jogo atualizado com sucesso.";
            return RedirectToAction(nameof(Index));
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "Falha ao atualizar jogo {JogoId}.", id);
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }

    public async Task<IActionResult> Delete(int id)
    {
        var jogo = await _jogos.ObterPorIdAsync(id);
        if (jogo is null)
            return NotFound();

        return View(jogo);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var sucesso = await _jogos.RemoverAsync(id);
            if (!sucesso)
                return NotFound();

            TempData["Sucesso"] = "Jogo removido com sucesso.";
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "Falha ao remover jogo {JogoId}.", id);
            TempData["Erro"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> BuscarGeneros(string texto)
    {
        if (string.IsNullOrWhiteSpace(texto) || texto.Trim().Length < TamanhoMinimoBusca)
            return Json(Array.Empty<object>());

        try
        {
            var generos = await _generos.BuscarPorNomeAsync(texto.Trim());
            return Json(generos.Select(g => new { id = g.Id, nome = g.Nome }));
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "Falha ao buscar gêneros.");
            return Json(Array.Empty<object>());
        }
    }

    [HttpGet]
    public async Task<IActionResult> BuscarDesenvolvedores(string texto)
    {
        if (string.IsNullOrWhiteSpace(texto) || texto.Trim().Length < TamanhoMinimoBusca)
            return Json(Array.Empty<object>());

        try
        {
            var desenvolvedores = await _desenvolvedores.BuscarPorNomeAsync(texto.Trim());
            return Json(desenvolvedores.Select(d => new { id = d.Id, nome = d.Nome }));
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "Falha ao buscar desenvolvedores.");
            return Json(Array.Empty<object>());
        }
    }

    [HttpGet]
    public async Task<IActionResult> BuscarPublicadoras(string texto)
    {
        if (string.IsNullOrWhiteSpace(texto) || texto.Trim().Length < TamanhoMinimoBusca)
            return Json(Array.Empty<object>());

        try
        {
            var publicadoras = await _publicadoras.BuscarPorNomeAsync(texto.Trim());
            return Json(publicadoras.Select(p => new { id = p.Id, nome = p.Nome }));
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "Falha ao buscar publicadoras.");
            return Json(Array.Empty<object>());
        }
    }
}
