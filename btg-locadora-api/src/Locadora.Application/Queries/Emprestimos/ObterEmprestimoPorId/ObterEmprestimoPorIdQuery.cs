using Locadora.Application.Dtos;
using MediatR;

namespace Locadora.Application.Queries.Emprestimos.ObterEmprestimoPorId;

public record ObterEmprestimoPorIdQuery(int Id) : IRequest<EmprestimoDto?>;
