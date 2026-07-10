using Locadora.Application.Dtos;
using Locadora.Application.Commands.Autenticacao.Autenticar;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Locadora.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AutenticacaoController : ControllerBase
{
    private readonly ISender _sender;

    public AutenticacaoController(ISender sender)
    {
        _sender = sender;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(typeof(TokenDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TokenDto>> Login(LoginInputDto input)
    {
        var token = await _sender.Send(new AutenticarCommand(input));
        return token is null ? Unauthorized() : Ok(token);
    }
}
