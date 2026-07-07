namespace Locadora.Application.Autenticacao;

public class JwtSettings
{
    public const string Secao = "Jwt";

    public string SecretKey { get; set; } = string.Empty;
    public int ExpirationMinutes { get; set; } = 60;
}
