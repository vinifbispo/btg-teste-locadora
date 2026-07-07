namespace Locadora.Application.Dtos;

public class LoginInputDto
{
    public string Usuario { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
}

public class TokenDto
{
    public string AccessToken { get; set; } = string.Empty;
    public DateTime ExpiraEm { get; set; }
}
