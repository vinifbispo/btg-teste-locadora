using Locadora.Web.Application.Interfaces;
using Locadora.Web.Domain.Exceptions;
using Locadora.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Locadora.Web.Controllers;

public class AmigosController : Controller
{
    private readonly IAmigoService _amigos;
    private readonly ILogger<AmigosController> _logger;

    public AmigosController(IAmigoService amigos, ILogger<AmigosController> logger)
    {
        _amigos = amigos;
        _logger = logger;
    }

    public async Task<IActionResult> Index(string? busca, int page = 1)
    {
        var resultado = await _amigos.ListarAsync(busca, page);

        var model = new ListaPaginadaViewModel<Domain.Models.Amigo>
        {
            Items = resultado.Items,
            Busca = busca,
            Page = resultado.Page,
            PageSize = resultado.PageSize,
            TotalCount = resultado.TotalCount,
            TotalPages = resultado.TotalPages
        };

        return View(model);
    }

    public async Task<IActionResult> Details(int id)
    {
        var amigo = await _amigos.ObterPorIdAsync(id);
        if (amigo is null)
            return NotFound();

        return View(amigo);
    }

    public IActionResult Create() => View(new AmigoFormViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AmigoFormViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var amigo = await _amigos.CriarAsync(model.ToInput(), model.ChaveIdempotencia);
            TempData["Sucesso"] = $"Amigo \"{amigo.Nome} {amigo.Sobrenome}\" cadastrado com sucesso.";
            return RedirectToAction(nameof(Index));
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "Falha ao criar amigo.");
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }

    public async Task<IActionResult> Edit(int id)
    {
        var amigo = await _amigos.ObterPorIdAsync(id);
        if (amigo is null)
            return NotFound();

        return View(AmigoFormViewModel.FromAmigo(amigo));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, AmigoFormViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var sucesso = await _amigos.AtualizarAsync(id, model.ToInput());
            if (!sucesso)
                return NotFound();

            TempData["Sucesso"] = "Amigo atualizado com sucesso.";
            return RedirectToAction(nameof(Index));
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "Falha ao atualizar amigo {AmigoId}.", id);
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }

    public async Task<IActionResult> Delete(int id)
    {
        var amigo = await _amigos.ObterPorIdAsync(id);
        if (amigo is null)
            return NotFound();

        return View(amigo);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var sucesso = await _amigos.RemoverAsync(id);
            if (!sucesso)
                return NotFound();

            TempData["Sucesso"] = "Amigo removido com sucesso.";
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "Falha ao remover amigo {AmigoId}.", id);
            TempData["Erro"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }
}
