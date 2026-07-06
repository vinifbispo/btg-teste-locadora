namespace Locadora.Infrastructure.Caching;

public class CacheSettings
{
    public const string Secao = "Cache";

    public int ExpirationMinutes { get; set; } = 10;

    public int TimeoutMilliseconds { get; set; } = 2000;
}
