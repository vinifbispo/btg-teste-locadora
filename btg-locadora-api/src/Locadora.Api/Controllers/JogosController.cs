using Locadora.Application.Dtos;
using Locadora.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Locadora.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class JogosController : ControllerBase
{
    private readonly IJogoService _jogos;

    public JogosController(IJogoService jogos)
    {
        _jogos = jogos;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<JogoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<JogoDto>>> GetJogos([FromQuery] string? busca)
    {
        var jogos = await _jogos.ListarAsync(busca);
        return Ok(jogos);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(JogoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<JogoDto>> GetJogo(int id)
    {
        var jogo = await _jogos.ObterPorIdAsync(id);
        return jogo is null ? NotFound() : Ok(jogo);
    }

    [HttpPost]
    [ProducesResponseType(typeof(JogoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<JogoDto>> CreateJogo(
        JogoInputDto input,
        [FromHeader(Name = "Idempotency-Key")] string? chaveIdempotencia)
    {
        var jogo = await _jogos.CriarAsync(input, chaveIdempotencia);
        return CreatedAtAction(nameof(GetJogo), new { id = jogo.Id }, jogo);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateJogo(int id, JogoInputDto input)
    {
        var atualizado = await _jogos.AtualizarAsync(id, input);
        return atualizado ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteJogo(int id)
    {
        var removido = await _jogos.RemoverAsync(id);
        return removido ? NoContent() : NotFound();
    }
}
