using Locadora.Application.Dtos;
using Locadora.Application.Commands.Jogos.AtualizarJogo;
using Locadora.Application.Commands.Jogos.CriarJogo;
using Locadora.Application.Commands.Jogos.RemoverJogo;
using Locadora.Application.Queries.Jogos.ListarJogos;
using Locadora.Application.Queries.Jogos.ObterJogoPorId;
using Locadora.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Locadora.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class JogosController : ControllerBase
{
    private readonly ISender _sender;

    public JogosController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResultDto<JogoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResultDto<JogoDto>>> GetJogos([FromQuery] string? busca, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var jogos = await _sender.Send(new ListarJogosQuery(busca, page, pageSize));
        return Ok(jogos);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(JogoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<JogoDto>> GetJogo(int id)
    {
        var jogo = await _sender.Send(new ObterJogoPorIdQuery(id));
        return jogo is null ? NotFound() : Ok(jogo);
    }

    [HttpPost]
    [ProducesResponseType(typeof(JogoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<JogoDto>> CreateJogo(
        JogoInputDto input,
        [FromHeader(Name = "Idempotency-Key")] string? chaveIdempotencia)
    {
        try
        {
            var jogo = await _sender.Send(new CriarJogoCommand(input, chaveIdempotencia));
            return CreatedAtAction(nameof(GetJogo), new { id = jogo.Id }, jogo);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateJogo(int id, JogoInputDto input)
    {
        try
        {
            var atualizado = await _sender.Send(new AtualizarJogoCommand(id, input));
            return atualizado ? NoContent() : NotFound();
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteJogo(int id)
    {
        try
        {
            var removido = await _sender.Send(new RemoverJogoCommand(id));
            return removido ? NoContent() : NotFound();
        }
        catch (ConflitoException ex)
        {
            return Conflict(new { mensagem = ex.Message });
        }
    }
}
