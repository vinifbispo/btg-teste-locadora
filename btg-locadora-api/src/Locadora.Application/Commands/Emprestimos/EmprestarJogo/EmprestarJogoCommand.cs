using Locadora.Application.Dtos;
using MediatR;

namespace Locadora.Application.Commands.Emprestimos.EmprestarJogo;

public record EmprestarJogoCommand(EmprestimoInputDto Input, string? ChaveIdempotencia) : IRequest<EmprestimoDto>;
