using System.Net;
using System.Text;
using Locadora.Web.Application.Interfaces;
using Locadora.Web.Domain.Models;
using Locadora.Web.Infrastructure.Http.Extensions;
using Newtonsoft.Json;

namespace Locadora.Web.Infrastructure.Http;

public class EmprestimoApiClient : IEmprestimoApiClient
{
    private const string Endpoint = "api/Emprestimos";

    private readonly HttpClient _http;

    public EmprestimoApiClient(HttpClient http) => _http = http;

    public async Task<PagedResult<Emprestimo>> ListarAsync(int? jogoId = null, int? amigoId = null, bool? apenasAtivos = null, int page = 1, int pageSize = 10, CancellationToken ct = default)
    {
        var query = new List<string> { $"page={page}", $"pageSize={pageSize}" };
        if (jogoId is > 0) query.Add($"jogoId={jogoId}");
        if (amigoId is > 0) query.Add($"amigoId={amigoId}");
        if (apenasAtivos is not null) query.Add($"apenasAtivos={apenasAtivos.Value.ToString().ToLowerInvariant()}");

        var url = $"{Endpoint}?{string.Join("&", query)}";

        _http.DefaultRequestHeaders.Clear();
        _http.SetMediaJson();

        var response = await _http.GetAsync(url, ct);
        await ApiResponseHandler.GarantirSucessoAsync(response, ct);

        var corpo = await response.Content.ReadAsStringAsync(ct);
        return JsonConvert.DeserializeObject<PagedResult<Emprestimo>>(corpo) ?? new PagedResult<Emprestimo>();
    }

    public async Task<Emprestimo?> ObterPorIdAsync(int id, CancellationToken ct = default)
    {
        _http.DefaultRequestHeaders.Clear();
        _http.SetMediaJson();

        var response = await _http.GetAsync($"{Endpoint}/{id}", ct);
        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        await ApiResponseHandler.GarantirSucessoAsync(response, ct);

        var corpo = await response.Content.ReadAsStringAsync(ct);
        return JsonConvert.DeserializeObject<Emprestimo>(corpo);
    }

    public async Task<Emprestimo> CriarAsync(EmprestimoInput input, string? chaveIdempotencia = null, CancellationToken ct = default)
    {
        _http.DefaultRequestHeaders.Clear();
        _http.SetMediaJson();
        if (!string.IsNullOrWhiteSpace(chaveIdempotencia))
            _http.DefaultRequestHeaders.Add("Idempotency-Key", chaveIdempotencia);

        var conteudo = new StringContent(JsonConvert.SerializeObject(input), Encoding.UTF8, "application/json");
        var response = await _http.PostAsync(Endpoint, conteudo, ct);
        await ApiResponseHandler.GarantirSucessoAsync(response, ct);

        var corpo = await response.Content.ReadAsStringAsync(ct);
        return JsonConvert.DeserializeObject<Emprestimo>(corpo)!;
    }

    public async Task<bool> DevolverAsync(int id, CancellationToken ct = default)
    {
        _http.DefaultRequestHeaders.Clear();
        _http.SetMediaJson();

        var response = await _http.PutAsync($"{Endpoint}/{id}/devolucao", null, ct);
        if (response.StatusCode == HttpStatusCode.NotFound)
            return false;

        await ApiResponseHandler.GarantirSucessoAsync(response, ct);
        return true;
    }
}
