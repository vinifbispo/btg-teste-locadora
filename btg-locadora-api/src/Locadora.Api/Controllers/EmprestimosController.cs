using Locadora.Application.Dtos;
using Locadora.Application.Interfaces;
using Locadora.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Locadora.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class EmprestimosController : ControllerBase
{
    private readonly IEmprestimoService _emprestimos;

    public EmprestimosController(IEmprestimoService emprestimos)
    {
        _emprestimos = emprestimos;
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
        var emprestimos = await _emprestimos.ListarAsync(jogoId, amigoId, apenasAtivos, page, pageSize);
        return Ok(emprestimos);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(EmprestimoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EmprestimoDto>> GetEmprestimo(int id)
    {
        var emprestimo = await _emprestimos.ObterPorIdAsync(id);
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
            var emprestimo = await _emprestimos.EmprestarAsync(input, chaveIdempotencia);
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
        var devolvido = await _emprestimos.DevolverAsync(id);
        return devolvido ? NoContent() : NotFound();
    }
}
