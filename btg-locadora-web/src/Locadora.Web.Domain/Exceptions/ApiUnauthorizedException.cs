namespace Locadora.Web.Domain.Exceptions;

public class ApiUnauthorizedException : Exception
{
    public ApiUnauthorizedException(string message) : base(message)
    {
    }
}
