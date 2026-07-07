using System.Net;
using System.Text;
using FluentAssertions;
using Locadora.Web.Domain.Exceptions;
using Locadora.Web.Domain.Models;
using Locadora.Web.Infrastructure.Http;
using Newtonsoft.Json;

namespace Locadora.Web.Infrastructure.UnitTest.Http;

public class JogoApiClientTests
{
    [Fact]
    public async Task ListarAsync_DeveMontarQueryStringEDesserializarResposta()
    {
        var esperado = new PagedResult<Jogo>
        {
            Items = new List<Jogo> { new() { Id = 1, Nome = "The Witcher 3" } },
            Page = 2,
            PageSize = 5,
            TotalCount = 1,
            TotalPages = 1
        };
        var handler = new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonConvert.SerializeObject(esperado), Encoding.UTF8, "application/json")
        });
        var client = new JogoApiClient(new HttpClient(handler) { BaseAddress = new Uri("https://api.teste.local/") });

        var resultado = await client.ListarAsync("witcher", 2, 5);

        resultado.Should().BeEquivalentTo(esperado);
        handler.UltimaRequisicao!.RequestUri!.PathAndQuery.Should().Be("/api/Jogos?page=2&pageSize=5&busca=witcher");
    }

    [Fact]
    public async Task ObterPorIdAsync_QuandoRespostaEh404_DeveRetornarNull()
    {
        var handler = new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.NotFound));
        var client = new JogoApiClient(new HttpClient(handler) { BaseAddress = new Uri("https://api.teste.local/") });

        var resultado = await client.ObterPorIdAsync(99);

        resultado.Should().BeNull();
    }

    [Fact]
    public async Task CriarAsync_DeveEnviarChaveIdempotenciaECorpoSerializado()
    {
        var criado = new Jogo { Id = 5, Nome = "Elden Ring" };
        var handler = new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(JsonConvert.SerializeObject(criado), Encoding.UTF8, "application/json")
        });
        var client = new JogoApiClient(new HttpClient(handler) { BaseAddress = new Uri("https://api.teste.local/") });

        var resultado = await client.CriarAsync(new JogoInput { Nome = "Elden Ring" }, "chave-123");

        resultado.Should().BeEquivalentTo(criado);
        handler.UltimaRequisicao!.Headers.GetValues("Idempotency-Key").Should().ContainSingle("chave-123");
        handler.UltimoCorpoRequisicao.Should().Contain("Elden Ring");
    }

    [Fact]
    public async Task CriarAsync_QuandoApiRetornaErroComMensagem_DeveLancarApiExceptionComAMensagem()
    {
        var handler = new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent("{\"mensagem\":\"Nome é obrigatório.\"}", Encoding.UTF8, "application/json")
        });
        var client = new JogoApiClient(new HttpClient(handler) { BaseAddress = new Uri("https://api.teste.local/") });

        var acao = () => client.CriarAsync(new JogoInput());

        var excecao = await acao.Should().ThrowAsync<ApiException>();
        excecao.Which.Message.Should().Be("Nome é obrigatório.");
        excecao.Which.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task CriarAsync_QuandoApiRetorna401_DeveLancarApiUnauthorizedException()
    {
        var handler = new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.Unauthorized));
        var client = new JogoApiClient(new HttpClient(handler) { BaseAddress = new Uri("https://api.teste.local/") });

        var acao = () => client.CriarAsync(new JogoInput { Nome = "Elden Ring" });

        await acao.Should().ThrowAsync<ApiUnauthorizedException>();
    }

    [Fact]
    public async Task AtualizarAsync_QuandoRespostaEh404_DeveRetornarFalso()
    {
        var handler = new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.NotFound));
        var client = new JogoApiClient(new HttpClient(handler) { BaseAddress = new Uri("https://api.teste.local/") });

        var resultado = await client.AtualizarAsync(1, new JogoInput { Nome = "Jogo Atualizado" });

        resultado.Should().BeFalse();
    }

    [Fact]
    public async Task RemoverAsync_QuandoRespostaEhSucesso_DeveRetornarVerdadeiro()
    {
        var handler = new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.NoContent));
        var client = new JogoApiClient(new HttpClient(handler) { BaseAddress = new Uri("https://api.teste.local/") });

        var resultado = await client.RemoverAsync(1);

        resultado.Should().BeTrue();
    }
}
