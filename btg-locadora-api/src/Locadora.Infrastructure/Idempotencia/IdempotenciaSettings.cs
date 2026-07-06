namespace Locadora.Infrastructure.Idempotencia;

public class IdempotenciaSettings
{
    public const string Secao = "Idempotencia";

    public int ExpirationHours { get; set; } = 24;
}
