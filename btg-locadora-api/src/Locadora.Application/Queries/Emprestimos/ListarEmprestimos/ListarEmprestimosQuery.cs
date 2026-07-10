using Locadora.Application.Dtos;
using MediatR;

namespace Locadora.Application.Queries.Emprestimos.ListarEmprestimos;

public record ListarEmprestimosQuery(int? JogoId, int? AmigoId, bool? ApenasAtivos, int Page = 1, int PageSize = 10) : IRequest<PagedResultDto<EmprestimoDto>>;
