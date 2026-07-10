using Locadora.Application.Dtos;
using Locadora.Application.Queries.Generos.BuscarGenerosPorNome;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Locadora.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class GenerosController : ControllerBase
{
    private const int TamanhoMinimoBusca = 3;

    private readonly ISender _sender;

    public GenerosController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<GeneroDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<GeneroDto>>> GetGeneros([FromQuery] string busca)
    {
        if (string.IsNullOrWhiteSpace(busca) || busca.Trim().Length < TamanhoMinimoBusca)
            return BadRequest(new { mensagem = $"Informe ao menos {TamanhoMinimoBusca} letras para realizar a busca." });

        var generos = await _sender.Send(new BuscarGenerosPorNomeQuery(busca.Trim()));
        return Ok(generos);
    }
}
