using Locadora.Web.Domain.Exceptions;
using Locadora.Web.Infrastructure.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Locadora.Web.Filters;

public class ApiExceptionFilter : IExceptionFilter
{
    private readonly ILogger<ApiExceptionFilter> _logger;
    private readonly ITempDataDictionaryFactory _tempDataFactory;
    private readonly IApiTokenProvider _tokenProvider;

    public ApiExceptionFilter(ILogger<ApiExceptionFilter> logger, ITempDataDictionaryFactory tempDataFactory, IApiTokenProvider tokenProvider)
    {
        _logger = logger;
        _tempDataFactory = tempDataFactory;
        _tokenProvider = tokenProvider;
    }

    public void OnException(ExceptionContext context)
    {
        switch (context.Exception)
        {
            case ApiUnauthorizedException apiUnauthorizedEx:
                _logger.LogError(apiUnauthorizedEx, "Falha ao autenticar a aplicação junto à API.");
                _tokenProvider.Invalidar();
                RedirecionarComErro(context, "Não foi possível autenticar com a API. Tente novamente em instantes.");
                break;

            case ApiException apiEx:
                _logger.LogError(apiEx, "Erro retornado pela API.");
                RedirecionarComErro(context, apiEx.Message);
                break;

            case HttpRequestException httpEx:
                _logger.LogError(httpEx, "Falha de comunicação com a API.");
                RedirecionarComErro(context, "Não foi possível se comunicar com a API. Verifique se ela está em execução.");
                break;

            case TaskCanceledException taskEx:
                _logger.LogError(taskEx, "Tempo limite excedido ao comunicar com a API.");
                RedirecionarComErro(context, "A API demorou muito para responder. Tente novamente.");
                break;
        }
    }

    private void RedirecionarComErro(ExceptionContext context, string mensagem)
    {
        var tempData = _tempDataFactory.GetTempData(context.HttpContext);
        tempData["Erro"] = mensagem;
        context.Result = new RedirectToActionResult("Index", "Home", null);
        context.ExceptionHandled = true;
    }
}
