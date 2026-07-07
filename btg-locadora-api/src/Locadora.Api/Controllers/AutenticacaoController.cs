using Locadora.Application.Dtos;
using Locadora.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Locadora.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AutenticacaoController : ControllerBase
{
    private readonly IAutenticacaoService _autenticacao;

    public AutenticacaoController(IAutenticacaoService autenticacao)
    {
        _autenticacao = autenticacao;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(typeof(TokenDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TokenDto>> Login(LoginInputDto input)
    {
        var token = await _autenticacao.AutenticarAsync(input);
        return token is null ? Unauthorized() : Ok(token);
    }
}
