using System.Net;
using System.Text;
using Locadora.Web.Application.Interfaces;
using Locadora.Web.Domain.Models;
using Locadora.Web.Infrastructure.Http.Extensions;
using Newtonsoft.Json;

namespace Locadora.Web.Infrastructure.Http;

public class AmigoApiClient : IAmigoApiClient
{
    private const string Endpoint = "api/Amigos";

    private readonly HttpClient _http;

    public AmigoApiClient(HttpClient http) => _http = http;

    public async Task<IEnumerable<Amigo>> ListarAsync(string? busca = null, CancellationToken ct = default)
    {
        var url = string.IsNullOrWhiteSpace(busca) ? Endpoint : $"{Endpoint}?busca={Uri.EscapeDataString(busca)}";

        _http.DefaultRequestHeaders.Clear();
        _http.SetMediaJson();

        var response = await _http.GetAsync(url, ct);
        await ApiResponseHandler.GarantirSucessoAsync(response, ct);

        var corpo = await response.Content.ReadAsStringAsync(ct);
        return JsonConvert.DeserializeObject<List<Amigo>>(corpo) ?? new List<Amigo>();
    }

    public async Task<Amigo?> ObterPorIdAsync(int id, CancellationToken ct = default)
    {
        _http.DefaultRequestHeaders.Clear();
        _http.SetMediaJson();

        var response = await _http.GetAsync($"{Endpoint}/{id}", ct);
        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        await ApiResponseHandler.GarantirSucessoAsync(response, ct);

        var corpo = await response.Content.ReadAsStringAsync(ct);
        return JsonConvert.DeserializeObject<Amigo>(corpo);
    }

    public async Task<Amigo> CriarAsync(AmigoInput input, string? chaveIdempotencia = null, CancellationToken ct = default)
    {
        _http.DefaultRequestHeaders.Clear();
        _http.SetMediaJson();
        if (!string.IsNullOrWhiteSpace(chaveIdempotencia))
            _http.DefaultRequestHeaders.Add("Idempotency-Key", chaveIdempotencia);

        var conteudo = new StringContent(JsonConvert.SerializeObject(input), Encoding.UTF8, "application/json");
        var response = await _http.PostAsync(Endpoint, conteudo, ct);
        await ApiResponseHandler.GarantirSucessoAsync(response, ct);

        var corpo = await response.Content.ReadAsStringAsync(ct);
        return JsonConvert.DeserializeObject<Amigo>(corpo)!;
    }

    public async Task<bool> AtualizarAsync(int id, AmigoInput input, CancellationToken ct = default)
    {
        _http.DefaultRequestHeaders.Clear();
        _http.SetMediaJson();

        var conteudo = new StringContent(JsonConvert.SerializeObject(input), Encoding.UTF8, "application/json");
        var response = await _http.PutAsync($"{Endpoint}/{id}", conteudo, ct);
        if (response.StatusCode == HttpStatusCode.NotFound)
            return false;

        await ApiResponseHandler.GarantirSucessoAsync(response, ct);
        return true;
    }

    public async Task<bool> RemoverAsync(int id, CancellationToken ct = default)
    {
        _http.DefaultRequestHeaders.Clear();
        _http.SetMediaJson();

        var response = await _http.DeleteAsync($"{Endpoint}/{id}", ct);
        if (response.StatusCode == HttpStatusCode.NotFound)
            return false;

        await ApiResponseHandler.GarantirSucessoAsync(response, ct);
        return true;
    }
}
