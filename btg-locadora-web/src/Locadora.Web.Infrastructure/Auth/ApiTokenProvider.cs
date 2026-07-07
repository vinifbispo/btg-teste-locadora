using Locadora.Web.Application.Interfaces;
using Locadora.Web.Domain.Models;
using Microsoft.Extensions.Options;

namespace Locadora.Web.Infrastructure.Auth;

public class ApiTokenProvider : IApiTokenProvider
{
    private readonly IAutenticacaoApiClient _autenticacaoApi;
    private readonly AutenticacaoApiOptions _options;
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private Token? _tokenAtual;

    public ApiTokenProvider(IAutenticacaoApiClient autenticacaoApi, IOptions<AutenticacaoApiOptions> options)
    {
        _autenticacaoApi = autenticacaoApi;
        _options = options.Value;
    }

    public async Task<string> ObterTokenAsync(CancellationToken ct = default)
    {
        if (TokenValido())
            return _tokenAtual!.AccessToken;

        await _semaphore.WaitAsync(ct);
        try
        {
            if (TokenValido())
                return _tokenAtual!.AccessToken;

            var input = new LoginInput { Usuario = _options.Usuario, Senha = _options.Senha };
            _tokenAtual = await _autenticacaoApi.LoginAsync(input, ct);
            return _tokenAtual.AccessToken;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public void Invalidar() => _tokenAtual = null;

    private bool TokenValido() => _tokenAtual is not null && _tokenAtual.ExpiraEm > DateTime.UtcNow.AddMinutes(1);
}
