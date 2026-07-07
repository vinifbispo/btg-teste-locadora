using Locadora.Application.Dtos;
using FluentValidation;

namespace Locadora.Api.Validators.Jogos;

public class JogoInputDtoValidator : AbstractValidator<JogoInputDto>
{
    public JogoInputDtoValidator()
    {
        RuleFor(p => p.Nome)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(p => p.GeneroIds)
            .NotEmpty();

        RuleForEach(p => p.GeneroIds)
            .GreaterThan(0);

        RuleFor(p => p.DesenvolvedorIds)
            .NotEmpty();

        RuleForEach(p => p.DesenvolvedorIds)
            .GreaterThan(0);

        RuleFor(p => p.PublicadoraIds)
            .NotEmpty();

        RuleForEach(p => p.PublicadoraIds)
            .GreaterThan(0);
    }
}
