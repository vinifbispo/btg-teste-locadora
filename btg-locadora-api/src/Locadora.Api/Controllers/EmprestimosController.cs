using Locadora.Application.Dtos;
using Locadora.Application.Commands.Emprestimos.DevolverEmprestimo;
using Locadora.Application.Commands.Emprestimos.EmprestarJogo;
using Locadora.Application.Queries.Emprestimos.ListarEmprestimos;
using Locadora.Application.Queries.Emprestimos.ObterEmprestimoPorId;
using Locadora.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Locadora.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class EmprestimosController : ControllerBase
{
    private readonly ISender _sender;

    public EmprestimosController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResultDto<EmprestimoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResultDto<EmprestimoDto>>> GetEmprestimos(
        [FromQuery] int? jogoId,
        [FromQuery] int? amigoId,
        [FromQuery] bool? apenasAtivos,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var emprestimos = await _sender.Send(new ListarEmprestimosQuery(jogoId, amigoId, apenasAtivos, page, pageSize));
        return Ok(emprestimos);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(EmprestimoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EmprestimoDto>> GetEmprestimo(int id)
    {
        var emprestimo = await _sender.Send(new ObterEmprestimoPorIdQuery(id));
        return emprestimo is null ? NotFound() : Ok(emprestimo);
    }

    [HttpPost]
    [ProducesResponseType(typeof(EmprestimoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<EmprestimoDto>> CreateEmprestimo(
        EmprestimoInputDto input,
        [FromHeader(Name = "Idempotency-Key")] string? chaveIdempotencia)
    {
        try
        {
            var emprestimo = await _sender.Send(new EmprestarJogoCommand(input, chaveIdempotencia));
            return CreatedAtAction(nameof(GetEmprestimo), new { id = emprestimo.Id }, emprestimo);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
        catch (ConflitoException ex)
        {
            return Conflict(new { mensagem = ex.Message });
        }
    }

    [HttpPut("{id:int}/devolucao")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DevolverEmprestimo(int id)
    {
        var devolvido = await _sender.Send(new DevolverEmprestimoCommand(id));
        return devolvido ? NoContent() : NotFound();
    }
}
