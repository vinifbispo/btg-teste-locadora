using Locadora.Application.Dtos;
using FluentValidation;

namespace Locadora.Api.Validators.Amigos;

public class AmigoInputDtoValidator : AbstractValidator<AmigoInputDto>
{
    public AmigoInputDtoValidator()
    {
        RuleFor(p => p.Nome)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(p => p.Sobrenome)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(p => p.Idade)
            .GreaterThan(0)
            .LessThanOrEqualTo(120);
    }
}
