using Locadora.Application.Dtos;
using Locadora.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Locadora.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class AmigosController : ControllerBase
{
    private readonly IAmigoService _amigos;

    public AmigosController(IAmigoService amigos)
    {
        _amigos = amigos;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResultDto<AmigoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResultDto<AmigoDto>>> GetAmigos([FromQuery] string? busca, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var amigos = await _amigos.ListarAsync(busca, page, pageSize);
        return Ok(amigos);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(AmigoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AmigoDto>> GetAmigo(int id)
    {
        var amigo = await _amigos.ObterPorIdAsync(id);
        return amigo is null ? NotFound() : Ok(amigo);
    }

    [HttpPost]
    [ProducesResponseType(typeof(AmigoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AmigoDto>> CreateAmigo(
        AmigoInputDto input,
        [FromHeader(Name = "Idempotency-Key")] string? chaveIdempotencia)
    {
        var amigo = await _amigos.CriarAsync(input, chaveIdempotencia);
        return CreatedAtAction(nameof(GetAmigo), new { id = amigo.Id }, amigo);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAmigo(int id, AmigoInputDto input)
    {
        var atualizado = await _amigos.AtualizarAsync(id, input);
        return atualizado ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAmigo(int id)
    {
        var removido = await _amigos.RemoverAsync(id);
        return removido ? NoContent() : NotFound();
    }
}
