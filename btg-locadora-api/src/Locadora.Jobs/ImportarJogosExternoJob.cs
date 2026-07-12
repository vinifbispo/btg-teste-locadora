using Hangfire;
using Locadora.Application.Commands.Jogos.ImportarJogosExterno;
using MediatR;

namespace Locadora.Jobs;

public class ImportarJogosExternoJob
{
    private readonly ISender _sender;

    public ImportarJogosExternoJob(ISender sender)
    {
        _sender = sender;
    }

    [AutomaticRetry(Attempts = 3)]
    public Task ExecutarAsync(CancellationToken cancellationToken)
        => _sender.Send(new ImportarJogosExternoCommand(), cancellationToken);
}
