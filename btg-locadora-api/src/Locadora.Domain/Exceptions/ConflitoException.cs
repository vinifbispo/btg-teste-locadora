namespace Locadora.Domain.Exceptions;

public class ConflitoException : Exception
{
    public ConflitoException(string message) : base(message)
    {
    }
}
