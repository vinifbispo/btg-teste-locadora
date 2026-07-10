using Locadora.Domain.Caching;
using Locadora.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Locadora.Application.Commands.Amigos.AtualizarAmigo;

public class AtualizarAmigoCommandHandler : IRequestHandler<AtualizarAmigoCommand, bool>
{
    private readonly IAmigoRepository _amigos;
    private readonly IAmigoCache _cache;
    private readonly ILogger<AtualizarAmigoCommandHandler> _logger;

    public AtualizarAmigoCommandHandler(IAmigoRepository amigos, IAmigoCache cache, ILogger<AtualizarAmigoCommandHandler> logger)
    {
        _amigos = amigos;
        _cache = cache;
        _logger = logger;
    }

    public async Task<bool> Handle(AtualizarAmigoCommand request, CancellationToken cancellationToken)
    {
        var amigo = await _amigos.ObterPorIdAsync(request.Id);
        if (amigo is null)
        {
            _logger.LogWarning("Atualização falhou: amigo {AmigoId} não encontrado.", request.Id);
            return false;
        }

        var input = request.Input;
        amigo.Nome = input.Nome;
        amigo.Sobrenome = input.Sobrenome;
        amigo.Idade = input.Idade;
        amigo.DataAtualizacao = DateTime.UtcNow;

        await _amigos.AtualizarAsync(amigo);
        await _cache.DefinirAsync(amigo);

        _logger.LogInformation("Amigo {AmigoId} atualizado.", request.Id);
        return true;
    }
}
