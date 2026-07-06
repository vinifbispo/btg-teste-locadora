using System.Text.Json;
using Locadora.Domain.Idempotencia;

namespace Locadora.Application.Idempotencia;

internal static class ExecutorIdempotente
{
    public static async Task<TDto?> ObterAsync<TDto>(IArmazenamentoIdempotencia store, string escopo, string? chaveIdempotencia)
    {
        if (string.IsNullOrWhiteSpace(chaveIdempotencia))
            return default;

        var existente = await store.ObterAsync(Chave(escopo, chaveIdempotencia));
        return existente is null ? default : JsonSerializer.Deserialize<TDto>(existente);
    }

    public static async Task SalvarAsync<TDto>(IArmazenamentoIdempotencia store, string escopo, string? chaveIdempotencia, TDto resultado)
    {
        if (string.IsNullOrWhiteSpace(chaveIdempotencia))
            return;

        await store.ArmazenarAsync(Chave(escopo, chaveIdempotencia), JsonSerializer.Serialize(resultado));
    }

    private static string Chave(string escopo, string chaveIdempotencia) => $"{escopo}:{chaveIdempotencia}";
}
