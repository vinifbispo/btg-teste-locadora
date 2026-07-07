using System.Net;
using System.Text;
using FluentAssertions;
using Locadora.Web.Domain.Models;
using Locadora.Web.Infrastructure.Http;
using Newtonsoft.Json;

namespace Locadora.Web.Infrastructure.UnitTest.Http;

public class EmprestimoApiClientTests
{
    [Fact]
    public async Task ListarAsync_DeveMontarQueryStringComFiltrosInformados()
    {
        var esperado = new PagedResult<Emprestimo>
        {
            Items = new List<Emprestimo> { new() { Id = 1, JogoId = 2, JogoNome = "Elden Ring", AmigoId = 3, AmigoNome = "Vinicius Bispo" } },
            Page = 1,
            PageSize = 10,
            TotalCount = 1,
            TotalPages = 1
        };
        var handler = new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonConvert.SerializeObject(esperado), Encoding.UTF8, "application/json")
        });
        var client = new EmprestimoApiClient(new HttpClient(handler) { BaseAddress = new Uri("https://api.teste.local/") });

        var resultado = await client.ListarAsync(jogoId: 2, amigoId: 3, apenasAtivos: true);

        resultado.Should().BeEquivalentTo(esperado);
        handler.UltimaRequisicao!.RequestUri!.PathAndQuery.Should().Be("/api/Emprestimos?page=1&pageSize=10&jogoId=2&amigoId=3&apenasAtivos=true");
    }

    [Fact]
    public async Task DevolverAsync_QuandoRespostaEhSucesso_DeveRetornarVerdadeiro()
    {
        var handler = new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK));
        var client = new EmprestimoApiClient(new HttpClient(handler) { BaseAddress = new Uri("https://api.teste.local/") });

        var resultado = await client.DevolverAsync(1);

        resultado.Should().BeTrue();
        handler.UltimaRequisicao!.Method.Should().Be(HttpMethod.Put);
        handler.UltimaRequisicao.RequestUri!.PathAndQuery.Should().Be("/api/Emprestimos/1/devolucao");
    }

    [Fact]
    public async Task DevolverAsync_QuandoRespostaEh404_DeveRetornarFalso()
    {
        var handler = new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.NotFound));
        var client = new EmprestimoApiClient(new HttpClient(handler) { BaseAddress = new Uri("https://api.teste.local/") });

        var resultado = await client.DevolverAsync(99);

        resultado.Should().BeFalse();
    }
}
