using Locadora.Application.Dtos;
using Locadora.Application.Idempotencia;
using Locadora.Application.Mappings;
using Locadora.Domain.Caching;
using Locadora.Domain.Entities;
using Locadora.Domain.Exceptions;
using Locadora.Domain.Idempotencia;
using Locadora.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Locadora.Application.Commands.Emprestimos.EmprestarJogo;

public class EmprestarJogoCommandHandler : IRequestHandler<EmprestarJogoCommand, EmprestimoDto>
{
    private readonly IEmprestimoRepository _emprestimos;
    private readonly IJogoRepository _jogos;
    private readonly IAmigoRepository _amigos;
    private readonly IEmprestimoCache _cache;
    private readonly IArmazenamentoIdempotencia _idempotencia;
    private readonly ILogger<EmprestarJogoCommandHandler> _logger;

    public EmprestarJogoCommandHandler(
        IEmprestimoRepository emprestimos,
        IJogoRepository jogos,
        IAmigoRepository amigos,
        IEmprestimoCache cache,
        IArmazenamentoIdempotencia idempotencia,
        ILogger<EmprestarJogoCommandHandler> logger)
    {
        _emprestimos = emprestimos;
        _jogos = jogos;
        _amigos = amigos;
        _cache = cache;
        _idempotencia = idempotencia;
        _logger = logger;
    }

    public async Task<EmprestimoDto> Handle(EmprestarJogoCommand request, CancellationToken cancellationToken)
    {
        var existente = await ExecutorIdempotente.ObterAsync<EmprestimoDto>(_idempotencia, "emprestimo:criar", request.ChaveIdempotencia);
        if (existente is not null)
        {
            _logger.LogInformation("Criação de empréstimo idempotente: retornando resultado já existente para a chave {ChaveIdempotencia}.", request.ChaveIdempotencia);
            return existente;
        }

        var input = request.Input;

        var jogo = await _jogos.ObterPorIdAsync(input.JogoId);
        if (jogo is null)
        {
            _logger.LogWarning("Empréstimo falhou: jogo {JogoId} não encontrado.", input.JogoId);
            throw new NotFoundException("Jogo não encontrado.");
        }

        var amigo = await _amigos.ObterPorIdAsync(input.AmigoId);
        if (amigo is null)
        {
            _logger.LogWarning("Empréstimo falhou: amigo {AmigoId} não encontrado.", input.AmigoId);
            throw new NotFoundException("Amigo não encontrado.");
        }

        var ativo = await _emprestimos.ObterAtivoPorJogoAsync(input.JogoId);
        if (ativo is not null)
        {
            _logger.LogWarning("Empréstimo falhou: jogo {JogoId} já está emprestado (empréstimo {EmprestimoId}).", input.JogoId, ativo.Id);
            throw new ConflitoException("Este jogo já está emprestado.");
        }

        var emprestimo = new Emprestimo
        {
            JogoId = jogo.Id,
            AmigoId = amigo.Id,
            DataEmprestimo = DateTime.UtcNow
        };

        await _emprestimos.AdicionarAsync(emprestimo);

        emprestimo.Jogo = jogo;
        emprestimo.Amigo = amigo;
        await _cache.DefinirAsync(emprestimo);

        _logger.LogInformation("Empréstimo {EmprestimoId} criado: jogo {JogoId} para amigo {AmigoId}.", emprestimo.Id, jogo.Id, amigo.Id);

        var dto = emprestimo.ToDto();
        await ExecutorIdempotente.SalvarAsync(_idempotencia, "emprestimo:criar", request.ChaveIdempotencia, dto);
        return dto;
    }
}
