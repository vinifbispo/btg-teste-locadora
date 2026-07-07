namespace Locadora.Web.Infrastructure.Auth;

public interface IApiTokenProvider
{
    Task<string> ObterTokenAsync(CancellationToken ct = default);
    void Invalidar();
}
