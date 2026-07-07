using System.Net.Http.Headers;

namespace Locadora.Web.Infrastructure.Http.Extensions;

internal static class HttpClientExtensions
{
    public static void SetMediaJson(this HttpClient client) =>
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
}
