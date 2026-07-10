using Locadora.Application.Dtos;
using Locadora.Application.Queries.Desenvolvedores.BuscarDesenvolvedoresPorNome;
using MediatR;
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

    private readonly ISender _sender;

    public DesenvolvedoresController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<DesenvolvedorDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<DesenvolvedorDto>>> GetDesenvolvedores([FromQuery] string busca)
    {
        if (string.IsNullOrWhiteSpace(busca) || busca.Trim().Length < TamanhoMinimoBusca)
            return BadRequest(new { mensagem = $"Informe ao menos {TamanhoMinimoBusca} letras para realizar a busca." });

        var desenvolvedores = await _sender.Send(new BuscarDesenvolvedoresPorNomeQuery(busca.Trim()));
        return Ok(desenvolvedores);
    }
}
