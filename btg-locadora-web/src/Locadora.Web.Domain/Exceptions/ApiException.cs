namespace Locadora.Web.Domain.Exceptions;

public class ApiException : Exception
{
    public int StatusCode { get; }
    public IDictionary<string, string[]>? Erros { get; }

    public ApiException(int statusCode, string message, IDictionary<string, string[]>? erros = null)
        : base(message)
    {
        StatusCode = statusCode;
        Erros = erros;
    }
}
