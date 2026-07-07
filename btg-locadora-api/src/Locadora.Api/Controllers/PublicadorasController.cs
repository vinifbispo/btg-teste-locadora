using Locadora.Application.Dtos;
using Locadora.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Locadora.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class PublicadorasController : ControllerBase
{
    private const int TamanhoMinimoBusca = 3;

    private readonly IPublicadoraService _publicadoras;

    public PublicadorasController(IPublicadoraService publicadoras)
    {
        _publicadoras = publicadoras;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PublicadoraDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<PublicadoraDto>>> GetPublicadoras([FromQuery] string busca)
    {
        if (string.IsNullOrWhiteSpace(busca) || busca.Trim().Length < TamanhoMinimoBusca)
            return BadRequest(new { mensagem = $"Informe ao menos {TamanhoMinimoBusca} letras para realizar a busca." });

        var publicadoras = await _publicadoras.BuscarPorNomeAsync(busca.Trim());
        return Ok(publicadoras);
    }
}
