using System.Net;
using System.Net.Http.Headers;
using Locadora.Web.Infrastructure.Auth;

namespace Locadora.Web.Infrastructure.Http.Handlers;

public class AuthorizationTokenHandler : DelegatingHandler
{
    private readonly IApiTokenProvider _tokenProvider;

    public AuthorizationTokenHandler(IApiTokenProvider tokenProvider)
    {
        _tokenProvider = tokenProvider;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _tokenProvider.ObterTokenAsync(cancellationToken);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
            _tokenProvider.Invalidar();

        return response;
    }
}
