using Locadora.Application.Dtos;
using Locadora.Application.Interfaces;
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

    private readonly IGeneroService _generos;

    public GenerosController(IGeneroService generos)
    {
        _generos = generos;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<GeneroDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<GeneroDto>>> GetGeneros([FromQuery] string busca)
    {
        if (string.IsNullOrWhiteSpace(busca) || busca.Trim().Length < TamanhoMinimoBusca)
            return BadRequest(new { mensagem = $"Informe ao menos {TamanhoMinimoBusca} letras para realizar a busca." });

        var generos = await _generos.BuscarPorNomeAsync(busca.Trim());
        return Ok(generos);
    }
}
