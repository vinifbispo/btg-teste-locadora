using Locadora.Application.Dtos;
using Locadora.Application.Idempotencia;
using Locadora.Application.Mappings;
using Locadora.Domain.Caching;
using Locadora.Domain.Entities;
using Locadora.Domain.Idempotencia;
using Locadora.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Locadora.Application.Commands.Amigos.CriarAmigo;

public class CriarAmigoCommandHandler : IRequestHandler<CriarAmigoCommand, AmigoDto>
{
    private readonly IAmigoRepository _amigos;
    private readonly IAmigoCache _cache;
    private readonly IArmazenamentoIdempotencia _idempotencia;
    private readonly ILogger<CriarAmigoCommandHandler> _logger;

    public CriarAmigoCommandHandler(
        IAmigoRepository amigos,
        IAmigoCache cache,
        IArmazenamentoIdempotencia idempotencia,
        ILogger<CriarAmigoCommandHandler> logger)
    {
        _amigos = amigos;
        _cache = cache;
        _idempotencia = idempotencia;
        _logger = logger;
    }

    public async Task<AmigoDto> Handle(CriarAmigoCommand request, CancellationToken cancellationToken)
    {
        var existente = await ExecutorIdempotente.ObterAsync<AmigoDto>(_idempotencia, "amigo:criar", request.ChaveIdempotencia);
        if (existente is not null)
        {
            _logger.LogInformation("Criação de amigo idempotente: retornando resultado já existente para a chave {ChaveIdempotencia}.", request.ChaveIdempotencia);
            return existente;
        }

        var input = request.Input;
        var agora = DateTime.UtcNow;
        var amigo = new Amigo
        {
            Nome = input.Nome,
            Sobrenome = input.Sobrenome,
            Idade = input.Idade,
            DataCadastro = agora,
            DataAtualizacao = agora
        };

        await _amigos.AdicionarAsync(amigo);
        await _cache.DefinirAsync(amigo);

        _logger.LogInformation("Amigo {AmigoId} criado: {Nome} {Sobrenome}.", amigo.Id, amigo.Nome, amigo.Sobrenome);

        var dto = amigo.ToDto();
        await ExecutorIdempotente.SalvarAsync(_idempotencia, "amigo:criar", request.ChaveIdempotencia, dto);
        return dto;
    }
}
