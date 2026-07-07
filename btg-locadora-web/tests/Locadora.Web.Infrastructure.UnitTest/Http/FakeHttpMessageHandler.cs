namespace Locadora.Web.Infrastructure.UnitTest.Http;

internal sealed class FakeHttpMessageHandler : HttpMessageHandler
{
    private readonly Func<HttpRequestMessage, HttpResponseMessage> _responder;

    public HttpRequestMessage? UltimaRequisicao { get; private set; }
    public string? UltimoCorpoRequisicao { get; private set; }

    public FakeHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> responder)
    {
        _responder = responder;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        UltimaRequisicao = request;
        UltimoCorpoRequisicao = request.Content is null
            ? null
            : await request.Content.ReadAsStringAsync(cancellationToken);

        return _responder(request);
    }
}
