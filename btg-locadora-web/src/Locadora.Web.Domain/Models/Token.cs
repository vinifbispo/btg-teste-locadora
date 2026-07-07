namespace Locadora.Web.Domain.Models;

public class Token
{
    public string AccessToken { get; set; } = string.Empty;
    public DateTime ExpiraEm { get; set; }
}
