using System.Net.Http.Json;
using System.Text.Json;
using Locadora.Domain.Integracoes;
using Locadora.Domain.Integracoes.Dtos;
using Locadora.Infrastructure.Http.Dtos;

namespace Locadora.Infrastructure.Http;

public class JogoExternoApiClient : IJogoExternoApiClient
{
    private const string Endpoint = "games";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _http;

    public JogoExternoApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<IEnumerable<JogoExterno>> ListarAsync(CancellationToken ct = default)
    {
        var resposta = await _http.GetFromJsonAsync<List<JogoExternoResponse>>(Endpoint, JsonOptions, ct)
            ?? new List<JogoExternoResponse>();

        return resposta.Select(r => new JogoExterno
        {
            Nome = r.Name,
            Generos = r.Genre,
            Desenvolvedores = r.Developers,
            Publicadoras = r.Publishers,
            DatasLancamento = r.ReleaseDates
        });
    }
}
