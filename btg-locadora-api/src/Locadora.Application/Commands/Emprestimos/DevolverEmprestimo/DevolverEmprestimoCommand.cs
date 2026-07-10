using MediatR;

namespace Locadora.Application.Commands.Emprestimos.DevolverEmprestimo;

public record DevolverEmprestimoCommand(int EmprestimoId) : IRequest<bool>;
