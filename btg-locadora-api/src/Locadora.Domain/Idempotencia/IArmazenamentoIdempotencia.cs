namespace Locadora.Domain.Idempotencia;

public interface IArmazenamentoIdempotencia
{
    Task<string?> ObterAsync(string chave);

    Task ArmazenarAsync(string chave, string valor);
}
