using System.Net;
using System.Text;
using Locadora.Web.Application.Interfaces;
using Locadora.Web.Domain.Models;
using Locadora.Web.Infrastructure.Http.Extensions;
using Newtonsoft.Json;

namespace Locadora.Web.Infrastructure.Http;

public class JogoApiClient : IJogoApiClient
{
    private const string Endpoint = "api/Jogos";

    private readonly HttpClient _http;

    public JogoApiClient(HttpClient http) => _http = http;

    public async Task<PagedResult<Jogo>> ListarAsync(string? busca = null, int page = 1, int pageSize = 10, CancellationToken ct = default)
    {
        var query = new List<string> { $"page={page}", $"pageSize={pageSize}" };
        if (!string.IsNullOrWhiteSpace(busca)) query.Add($"busca={Uri.EscapeDataString(busca)}");

        var url = $"{Endpoint}?{string.Join("&", query)}";

        _http.DefaultRequestHeaders.Clear();
        _http.SetMediaJson();

        var response = await _http.GetAsync(url, ct);
        await ApiResponseHandler.GarantirSucessoAsync(response, ct);

        var corpo = await response.Content.ReadAsStringAsync(ct);
        return JsonConvert.DeserializeObject<PagedResult<Jogo>>(corpo) ?? new PagedResult<Jogo>();
    }

    public async Task<Jogo?> ObterPorIdAsync(int id, CancellationToken ct = default)
    {
        _http.DefaultRequestHeaders.Clear();
        _http.SetMediaJson();

        var response = await _http.GetAsync($"{Endpoint}/{id}", ct);
        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        await ApiResponseHandler.GarantirSucessoAsync(response, ct);

        var corpo = await response.Content.ReadAsStringAsync(ct);
        return JsonConvert.DeserializeObject<Jogo>(corpo);
    }

    public async Task<Jogo> CriarAsync(JogoInput input, string? chaveIdempotencia = null, CancellationToken ct = default)
    {
        _http.DefaultRequestHeaders.Clear();
        _http.SetMediaJson();
        if (!string.IsNullOrWhiteSpace(chaveIdempotencia))
            _http.DefaultRequestHeaders.Add("Idempotency-Key", chaveIdempotencia);

        var conteudo = new StringContent(JsonConvert.SerializeObject(input), Encoding.UTF8, "application/json");
        var response = await _http.PostAsync(Endpoint, conteudo, ct);
        await ApiResponseHandler.GarantirSucessoAsync(response, ct);

        var corpo = await response.Content.ReadAsStringAsync(ct);
        return JsonConvert.DeserializeObject<Jogo>(corpo)!;
    }

    public async Task<bool> AtualizarAsync(int id, JogoInput input, CancellationToken ct = default)
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
