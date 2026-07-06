using Locadora.Application.Dtos;
using FluentValidation;

namespace Locadora.Api.Validators.Emprestimos;

public class EmprestimoInputDtoValidator : AbstractValidator<EmprestimoInputDto>
{
    public EmprestimoInputDtoValidator()
    {
        RuleFor(p => p.JogoId)
            .GreaterThan(0);

        RuleFor(p => p.AmigoId)
            .GreaterThan(0);
    }
}
