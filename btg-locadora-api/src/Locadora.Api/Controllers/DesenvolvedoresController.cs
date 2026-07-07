using Locadora.Application.Dtos;
using Locadora.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Locadora.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class DesenvolvedoresController : ControllerBase
{
    private const int TamanhoMinimoBusca = 3;

    private readonly IDesenvolvedorService _desenvolvedores;

    public DesenvolvedoresController(IDesenvolvedorService desenvolvedores)
    {
        _desenvolvedores = desenvolvedores;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<DesenvolvedorDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<DesenvolvedorDto>>> GetDesenvolvedores([FromQuery] string busca)
    {
        if (string.IsNullOrWhiteSpace(busca) || busca.Trim().Length < TamanhoMinimoBusca)
            return BadRequest(new { mensagem = $"Informe ao menos {TamanhoMinimoBusca} letras para realizar a busca." });

        var desenvolvedores = await _desenvolvedores.BuscarPorNomeAsync(busca.Trim());
        return Ok(desenvolvedores);
    }
}
