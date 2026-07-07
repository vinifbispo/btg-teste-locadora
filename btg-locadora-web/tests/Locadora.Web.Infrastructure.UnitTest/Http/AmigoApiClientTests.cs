using System.Net;
using System.Text;
using FluentAssertions;
using Locadora.Web.Domain.Models;
using Locadora.Web.Infrastructure.Http;
using Newtonsoft.Json;

namespace Locadora.Web.Infrastructure.UnitTest.Http;

public class AmigoApiClientTests
{
    [Fact]
    public async Task ListarAsync_DeveMontarQueryStringEDesserializarResposta()
    {
        var esperado = new PagedResult<Amigo>
        {
            Items = new List<Amigo> { new() { Id = 1, Nome = "Vinicius", Sobrenome = "Bispo" } },
            Page = 1,
            PageSize = 10,
            TotalCount = 1,
            TotalPages = 1
        };
        var handler = new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonConvert.SerializeObject(esperado), Encoding.UTF8, "application/json")
        });
        var client = new AmigoApiClient(new HttpClient(handler) { BaseAddress = new Uri("https://api.teste.local/") });

        var resultado = await client.ListarAsync("vinicius");

        resultado.Should().BeEquivalentTo(esperado);
        handler.UltimaRequisicao!.RequestUri!.PathAndQuery.Should().Be("/api/Amigos?page=1&pageSize=10&busca=vinicius");
    }

    [Fact]
    public async Task CriarAsync_DeveEnviarCorpoSerializadoERetornarAmigoCriado()
    {
        var criado = new Amigo { Id = 5, Nome = "Walanem", Sobrenome = "Figueiredo", Idade = 30 };
        var handler = new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(JsonConvert.SerializeObject(criado), Encoding.UTF8, "application/json")
        });
        var client = new AmigoApiClient(new HttpClient(handler) { BaseAddress = new Uri("https://api.teste.local/") });

        var resultado = await client.CriarAsync(new AmigoInput { Nome = "Walanem", Sobrenome = "Figueiredo", Idade = 30 });

        resultado.Should().BeEquivalentTo(criado);
        handler.UltimoCorpoRequisicao.Should().Contain("Walanem");
    }

    [Fact]
    public async Task RemoverAsync_QuandoRespostaEh404_DeveRetornarFalso()
    {
        var handler = new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.NotFound));
        var client = new AmigoApiClient(new HttpClient(handler) { BaseAddress = new Uri("https://api.teste.local/") });

        var resultado = await client.RemoverAsync(99);

        resultado.Should().BeFalse();
    }
}
