using System.Net;
using Locadora.Web.Domain.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Locadora.Web.Infrastructure.Http;

internal static class ApiResponseHandler
{
    public static async Task GarantirSucessoAsync(HttpResponseMessage response, CancellationToken ct)
    {
        if (response.IsSuccessStatusCode)
            return;

        if (response.StatusCode == HttpStatusCode.Unauthorized)
            throw new ApiUnauthorizedException("Sessão expirada ou inválida. Faça login novamente.");

        var corpo = await response.Content.ReadAsStringAsync(ct);
        var statusCode = (int)response.StatusCode;
        var (mensagem, erros) = ExtrairErro(corpo, response.ReasonPhrase, statusCode);

        throw new ApiException(statusCode, mensagem, erros);
    }

    private static (string Mensagem, IDictionary<string, string[]>? Erros) ExtrairErro(string corpo, string? reasonPhrase, int statusCode)
    {
        if (string.IsNullOrWhiteSpace(corpo))
            return ($"A API retornou {statusCode} ({reasonPhrase}).", null);

        try
        {
            var json = JObject.Parse(corpo);

            var mensagemSimples = json["mensagem"]?.ToString();
            if (!string.IsNullOrWhiteSpace(mensagemSimples))
                return (mensagemSimples, null);

            if (json["errors"] is JObject errosJson)
            {
                var erros = errosJson.Properties()
                    .ToDictionary(p => p.Name, p => p.Value?.ToObject<string[]>() ?? Array.Empty<string>());

                var mensagem = string.Join(" ", erros.SelectMany(e => e.Value));
                return (string.IsNullOrWhiteSpace(mensagem) ? "Dados inválidos." : mensagem, erros);
            }

            var title = json["title"]?.ToString();
            if (!string.IsNullOrWhiteSpace(title))
                return (title, null);
        }
        catch (JsonException)
        {
            // corpo não é JSON; usa mensagem padrão abaixo.
        }

        return ($"A API retornou {statusCode} ({reasonPhrase}). {corpo}", null);
    }
}
