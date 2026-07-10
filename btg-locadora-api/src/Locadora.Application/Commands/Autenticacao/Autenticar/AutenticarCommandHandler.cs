using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Locadora.Application.Autenticacao;
using Locadora.Application.Dtos;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Locadora.Application.Commands.Autenticacao.Autenticar;

public class AutenticarCommandHandler : IRequestHandler<AutenticarCommand, TokenDto?>
{
    private readonly JwtSettings _jwtSettings;
    private readonly AdminCredenciaisSettings _credenciais;
    private readonly ILogger<AutenticarCommandHandler> _logger;

    public AutenticarCommandHandler(IOptions<JwtSettings> jwtSettings, IOptions<AdminCredenciaisSettings> credenciais, ILogger<AutenticarCommandHandler> logger)
    {
        _jwtSettings = jwtSettings.Value;
        _credenciais = credenciais.Value;
        _logger = logger;
    }

    public Task<TokenDto?> Handle(AutenticarCommand request, CancellationToken cancellationToken)
    {
        var input = request.Input;

        if (input.Usuario != _credenciais.Usuario || input.Senha != _credenciais.Senha)
        {
            _logger.LogWarning("Falha de login para o usuário {Usuario}.", input.Usuario);
            return Task.FromResult<TokenDto?>(null);
        }

        var expiraEm = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, input.Usuario)
        };

        var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var credenciaisAssinatura = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: expiraEm,
            signingCredentials: credenciaisAssinatura);

        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

        _logger.LogInformation("Login bem-sucedido para o usuário {Usuario}.", input.Usuario);

        return Task.FromResult<TokenDto?>(new TokenDto
        {
            AccessToken = accessToken,
            ExpiraEm = expiraEm
        });
    }
}
