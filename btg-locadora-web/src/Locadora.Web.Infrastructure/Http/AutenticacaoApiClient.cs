using System.Net;
using System.Text;
using Locadora.Web.Application.Interfaces;
using Locadora.Web.Domain.Exceptions;
using Locadora.Web.Domain.Models;
using Locadora.Web.Infrastructure.Http.Extensions;
using Newtonsoft.Json;

namespace Locadora.Web.Infrastructure.Http;

public class AutenticacaoApiClient : IAutenticacaoApiClient
{
    private const string Endpoint = "api/Autenticacao";

    private readonly HttpClient _http;

    public AutenticacaoApiClient(HttpClient http) => _http = http;

    public async Task<Token> LoginAsync(LoginInput input, CancellationToken ct = default)
    {
        _http.DefaultRequestHeaders.Clear();
        _http.SetMediaJson();

        var conteudo = new StringContent(JsonConvert.SerializeObject(input), Encoding.UTF8, "application/json");
        var response = await _http.PostAsync($"{Endpoint}/login", conteudo, ct);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
            throw new ApiUnauthorizedException("Usuário ou senha inválidos.");

        await ApiResponseHandler.GarantirSucessoAsync(response, ct);

        var corpo = await response.Content.ReadAsStringAsync(ct);
        return JsonConvert.DeserializeObject<Token>(corpo)
            ?? throw new ApiException(500, "Resposta inválida da API de autenticação.");
    }
}
