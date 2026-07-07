using Locadora.Web.Application.Interfaces;
using Locadora.Web.Domain.Models;
using Locadora.Web.Infrastructure.Http.Extensions;
using Newtonsoft.Json;

namespace Locadora.Web.Infrastructure.Http;

public class DesenvolvedorApiClient : IDesenvolvedorApiClient
{
    private const string Endpoint = "api/Desenvolvedores";

    private readonly HttpClient _http;

    public DesenvolvedorApiClient(HttpClient http) => _http = http;

    public async Task<IEnumerable<Desenvolvedor>> BuscarPorNomeAsync(string busca, CancellationToken ct = default)
    {
        _http.DefaultRequestHeaders.Clear();
        _http.SetMediaJson();

        var response = await _http.GetAsync($"{Endpoint}?busca={Uri.EscapeDataString(busca)}", ct);
        await ApiResponseHandler.GarantirSucessoAsync(response, ct);

        var corpo = await response.Content.ReadAsStringAsync(ct);
        return JsonConvert.DeserializeObject<List<Desenvolvedor>>(corpo) ?? new List<Desenvolvedor>();
    }
}
