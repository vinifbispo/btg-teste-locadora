namespace Locadora.Application.Autenticacao;

public class AdminCredenciaisSettings
{
    public const string Secao = "AdminCredenciais";

    public string Usuario { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
}
