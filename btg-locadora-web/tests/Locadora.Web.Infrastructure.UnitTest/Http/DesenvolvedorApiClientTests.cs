using System.Net;
using System.Text;
using FluentAssertions;
using Locadora.Web.Domain.Models;
using Locadora.Web.Infrastructure.Http;
using Newtonsoft.Json;

namespace Locadora.Web.Infrastructure.UnitTest.Http;

public class DesenvolvedorApiClientTests
{
    [Fact]
    public async Task BuscarPorNomeAsync_DeveEnviarBuscaNaQueryStringEDesserializarResposta()
    {
        var esperado = new List<Desenvolvedor> { new() { Id = 1, Nome = "CD Projekt Red" } };
        var handler = new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonConvert.SerializeObject(esperado), Encoding.UTF8, "application/json")
        });
        var client = new DesenvolvedorApiClient(new HttpClient(handler) { BaseAddress = new Uri("https://api.teste.local/") });

        var resultado = await client.BuscarPorNomeAsync("cd projekt");

        resultado.Should().BeEquivalentTo(esperado);
        handler.UltimaRequisicao!.RequestUri!.PathAndQuery.Should().Be("/api/Desenvolvedores?busca=cd%20projekt");
    }
}
