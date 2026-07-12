using Hangfire;
using Hangfire.SqlServer;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Locadora.Jobs;

public static class HangfireInitializer
{
    public static async Task InitializeAsync(IConfiguration configuration, IBackgroundJobClient backgroundJobClient)
    {
        using var hangfireConnection = new SqlConnection(configuration.GetConnectionString("SqlServerDB"));

        try
        {
            await hangfireConnection.OpenAsync();
            SqlServerObjectsInstaller.Install(hangfireConnection);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                "Falha ao preparar o schema do Hangfire no SQL Server. Verifique a connection string 'SqlServerDB' e se o usuário do banco tem permissão para criar schema/tabelas (CREATE SCHEMA, CREATE TABLE).",
                ex);
        }

        backgroundJobClient.Enqueue<ImportarJogosExternoJob>(job => job.ExecutarAsync(CancellationToken.None));
    }
}
